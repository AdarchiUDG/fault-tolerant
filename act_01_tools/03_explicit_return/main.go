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