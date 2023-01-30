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
			fmt.Println()
			log.Println("Error:", err.Error())
			log.Println("Retrying")
			fmt.Println()
		}

		ranSuccessfuly = err == nil
	}

	fmt.Println("I run once the program finishes successfully")
}