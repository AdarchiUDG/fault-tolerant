using System.Text.Json;
using Spectre.Console;
using Timer = System.Timers.Timer;

var state = new State {
  User = null
};

if (File.Exists("._state_backup.json")) {
  state = JsonSerializer.Deserialize<State>(File.ReadAllText("._state_backup.json")) ?? state;
}

var users = new UsersContext();
var menu = new Option[] { Option.Add, Option.List, Option.Exit };
var option = Option.Add;

using var autoSaveTimer = new Timer {
  AutoReset = true,
  Interval = TimeSpan.FromSeconds(1).TotalMilliseconds,
  Enabled = true
};
autoSaveTimer.Elapsed += (_, _) => {
  File.WriteAllText("._state_backup.json", JsonSerializer.Serialize(state));
};

while (option != Option.Exit) {
  option = AnsiConsole.Prompt(new SelectionPrompt<Option>()
    .AddChoices(menu)
    .UseConverter(o => o switch {
      Option.Add => "Agregar usuarios",
      Option.List => "Listar usuarios",
      Option.Exit => "Salir",
      _ => ""
    }));

  switch (option) {
    case Option.Add:
      state.User ??= new User();
      users.PromptUser(state.User);
      users.Add(state.User);
      state.User = new User();
      
      File.WriteAllText(UsersContext.FilePath, JsonSerializer.Serialize(users.Users));
      break;
    case Option.List:
      var table = new Table();
      table.AddColumns("Id", "Email", "Password Hash");

      foreach (var user in users.Users) {
        table.AddRow(user.Id.ToString() ?? "<null>", user.Email ?? "<null>", user.Password ?? "<null>");
      }

      AnsiConsole.Write(table);
      break;
    case Option.Exit:
      break;
    default:
      throw new ArgumentOutOfRangeException();
  };
}

public enum Option {
  Add,
  List,
  Exit
}

public class UsersContext {
  public const string FilePath = "db.json";
  private List<User> _users;
  public IEnumerable<User> Users => _users.AsReadOnly();

  public UsersContext() {
    _users = new List<User>();

    if (File.Exists(FilePath)) {
      _users = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(FilePath)) ?? _users;
    }
  }

  public User PromptUser(User? user) {
    user ??= new User();

    if (user.Id is not null) {
      AnsiConsole.MarkupLine($"[green]Backup Id: {user.Id}[/]");
    }
    user.Id ??= AnsiConsole.Ask<int>("Id: ");
    
    if (user.Email is not null) {
      AnsiConsole.MarkupLine($"[green]Backup Email: {user.Email}[/]");
    }
    user.Email ??= AnsiConsole.Ask<string>("Email: ");

    if (user.Password is not null) {
      AnsiConsole.MarkupLine($"[green]Backup Password: {user.Email}[/]");
    }
    user.Password ??= BCrypt.Net.BCrypt.HashPassword(
      AnsiConsole.Prompt(
        new TextPrompt<string>("Password: ")
          .Secret()));

    return user;
  }

  public void Add(User user) {
    _users.Add(user);
  }
}