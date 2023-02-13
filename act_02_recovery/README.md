# Programa que sea capaz de restaurar el estado de ejecución.

Para esta práctica utilizaremos checkpointing para recuperar el estado de una aplicación ante un cierre inesperado de la aplicación.

Para esta práctica se utilizó el lenguaje C# y se realizo un pequeño ejemplo en el que capturaremos la información de un usuario.

La manera en que se estará haciendo el respaldo es mediante la clase `System.Timers.Timer`, la cual nos permitirá ejecutar un respaldo en un intervalo de tiempo definido.

El código para respaldar es el siguiente

```csharp
var autoSaveTimer = new Timer {
  AutoReset = true,
  Interval = TimeSpan.FromSeconds(1).TotalMilliseconds,
  Enabled = true
};
autoSaveTimer.Elapsed += (_, _) => {
  File.WriteAllText("._state_backup.json", JsonSerializer.Serialize(state));
};
```

Mientras que, para recuperar la información se carga en un objeto con la siguiente estructura y, al momento de solicitar la información se realizan validaciones para saltar la solicitud de los campos que ya tengan un valor.

```csharp
public class State {
  public User? User { get; set; }
}

[...]

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
```

---

## Aplicación

Al iniciar la aplicación se nos presentará con un menú, en el cual podremos seleccionar la opcion de agregar usuario.

```bash
❯ dotnet run
Id: 1
Email: correo@ejemplo.com
Password: 
```

Dentro de esta sección utilizaremos la combinación de teclas <kbd>CTRL</kbd> + <kbd>C</kbd> para parar la ejecución del programa de manera forzosa.

Volvemos a iniciar el programa y seleccionamos la opción de `Agregar usuario` y dentro de esa opción notaremos que los campos que habiamos rellenado son cargados y no se le solicitarán al usuario.

```bash
❯ dotnet run
Backup Id: 1
Backup Email: correo@ejemplo.com
Password: 
```