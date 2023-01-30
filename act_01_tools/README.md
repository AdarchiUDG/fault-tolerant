# Otras herramientas para el manejar errores

En esta práctica se explorarán otras maneras en las que es posible manejar y recuperarse de los errores.

---

## Indice

- [Promise - JavaScript](#promise---javascript)
  
- [Either - Rust](#either---rust)

- [Retorno Explicito - Go](#retorno-explicito---go)

- [Supervisión de crasheos - Go](#supervision-de-crasheos---go)

---

### Promise - JavaScript

**[`^        Regresar al inicio        ^`](#otras-herramientas-para-el-manejar-errores)**

Una de las maneras en que es posible manejar los errores en JavaScript es por medio de la API de Promises, esta API es usada cuando se realizan tareas asíncronas y provee con métodos para utilizar el resultado (`then`), manejar los errores (`catch`) y realizar una acción sin importar si la promesa fue exitosa o no (`finally`), cada uno de estos métodos tiene un equivalente en try, catch y finally respectivamente.

```js
import axios from 'axios'

axios.get("https://jsonplaceholder.typicode.com/todos/1")
    .then(value => {
        console.log("Got a response:")
        console.table(value.data)
    })
    .catch(reason => {
        console.error("Got an error:")
        console.error(reason.toString())
    })
    .finally(() => console.log("\nAlways prints"));
```

Ejemplo de respuesta exitosa

```bash
❯ node main.js
Got a response:
┌───────────┬──────────────────────┐
│  (index)  │        Values        │
├───────────┼──────────────────────┤
│  userId   │          1           │
│    id     │          1           │
│   title   │ 'delectus aut autem' │
│ completed │        false         │
└───────────┴──────────────────────┘

Always prints
```

Ejemplo de respuesta con error

```
❯ node main.js
Got an error:
Error: getaddrinfo ENOTFOUND jsonplaceholder.typicode.com

Always prints
```


---

### Either - Rust

**[`^        Regresar al inicio        ^`](#otras-herramientas-para-el-manejar-errores)**

En el caso de Rust, se presenta con el paradigma `Either`, esto permite al lenguaje entregar un objeto tipo `Result` el cual luego puede ser comparado con para saber si el valor es `Ok` o `Error`.

```rust
use serde_json;

async fn request(url: String) -> Result<serde_json::Value, reqwest::Error> {
    let request_result = reqwest::get(url)
        .await;

    let response = match request_result {
        Ok(r) => r,
        Err(e) => return Err(e),
    };

    let json_result = response.json::<serde_json::Value>()
        .await;

    json_result
}

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let json_result = request("https://jsonplaceholder.typicode.com/todos/1".to_string())
        .await;
    
    match json_result {
        Ok(r) => println!("{:#?}", r),
        Err(e) => println!("{}", e)
    };

    println!("Always prints");

    Ok(())
}
```

Ejemplo de respuesta exitosa

```bash
❯ cargo run
    Finished dev [unoptimized + debuginfo] target(s) in 0.52s
     Running `target/debug/either`
Object {
    "completed": Bool(false),
    "id": Number(1),
    "title": String("delectus aut autem"),
    "userId": Number(1),
}
Always prints
```

Ejemplo de respuesta con error

```bash
❯ cargo run
    Finished dev [unoptimized + debuginfo] target(s) in 0.09s
     Running `target/debug/either`
error sending request for url (https://jsonplaceholder.typicode.com/todos/1): error trying to connect: dns error: failed to lookup address information: nodename nor servname provided, or not known
Always prints
```

---

### Retorno Explicito - Go

**[`^        Regresar al inicio        ^`](#otras-herramientas-para-el-manejar-errores)**

El lenguaje go presenta otra manera para manejar los errores similar a la de Rust, en el cual, se espera que las funciones que puedan fallar regresen un puntero de tipo `Error` el cual contendrá el mensaje. Aunque el lenguaje permite ignorarlos utilizando el carácter `_` al momento de asignar el valor, lo propio es tomarlos en cuenta comparándolos con un valor `nil`.

```go
package main

import (
	"fmt"
	"io/ioutil"
	"log"
	"os"
)

func readFile(path string) (string, error) {
	file, err := os.Open(path)
	if err != nil {
		return "", err
	}

	contents, err := ioutil.ReadAll(file)
	if err != nil {
		return "", err
	}

	return string(contents), nil;
}

func main() {
	if len(os.Args) < 2 {
		log.Fatal("Expected file name as first parameter")
	}
	args := os.Args[1:]
	
	contents, err := readFile(args[0])

	if err != nil {
		log.Println("Error:", err.Error())
	} else {
		fmt.Println(contents)
	}

	fmt.Println("Always prints")
}
```

Ejemplo de respuesta exitosa

```bash
❯ go run ./main.go example.txt
Hello world!
Always prints
```

Ejemplo de respuesta fallida

```bash
2023/01/29 19:49:07 Error: open not_found.txt: no such file or directory
Always prints
```

---

<a id="supervision-de-crasheos---go"></a>

### Supervisión de crasheos - Go

**[`^        Regresar al inicio        ^`](#otras-herramientas-para-el-manejar-errores)**

Como ultima forma de manejar los errores, existe la forma de recuperarse de un crasheo inesperado, para esto, se utiliza un programa que será el encargado de ejecutar el programa que podría fallar y vigilará la ejecución, si el programa llega a tener un error y finaliza su ejecución con un código de salida diferente a `0`, entonces se volverá a ejecutar.

```go
package main

import (
	"fmt"
	"log"
	"os"
	"os/exec"
)

func prepareCommand() *exec.Cmd {
	cmd := exec.Command("python", "main.py")
	cmd.Stdin = os.Stdin
	cmd.Stdout = os.Stdout
	cmd.Stderr = os.Stderr

	return cmd
}

func main() {
	ranSuccessfuly := false
	for !ranSuccessfuly {
		cmd := prepareCommand()		

		err := cmd.Run()
		if err != nil {
			log.Println("Error:", err.Error())
			log.Println("Retrying")
			fmt.Println()
		}

		ranSuccessfuly = err == nil
	}

	fmt.Println("I run once the program finishes successfully")
}
```

Código del programa que se ejecutará

```python
def main():
    print("Divison")
    x = int(input("First number: "))
    y = int(input("Second number: "))

    print(x / y)

if __name__ == "__main__":
    main()
```

Ejemplo de salida

```bash
❯ go run ./main.go
Divison
First number: 10
Second number: 0
Traceback (most recent call last):
  File "$USER/act_01_tools/04_crash_supervisor/main.py", line 10, in <module>
    main()
  File "$USER/act_01_tools/04_crash_supervisor/main.py", line 7, in main
    print(x / y)
ZeroDivisionError: division by zero

2023/01/29 20:10:37 Error: exit status 1
2023/01/29 20:10:37 Retrying

Divison
First number: 10
Second number: 1
10.0
I run once the program finishes successfully
```

## Bibliografía

Warden, J. (2017, 3 de Noviembre). Error handling strategies. Medium. Consultado el 29 de Enero, 2023, en https://medium.com/@jesterxl/error-handling-strategies-b82d1b04f105 

MDN Web Docs. (2022, 13 de Diciembre). Promise - javascript: MDN. JavaScript | MDN. Consultado el 29 de Enero, 2023, en https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Promise 

Gerrand, A. (s.f.). The go blog. Go. Consultado el 29 de Enero, 2023, from https://go.dev/blog/error-handling-and-go 

Rust. (s.f.). The rust programming language. Recoverable Errors with Result - The Rust Programming Language. Consultado el 29 de Enero, 2023, en https://doc.rust-lang.org/book/ch09-02-recoverable-errors-with-result.html 
