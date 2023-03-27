package main

import (
	"fmt"
	"log"
	"sync"
)

func mul(x, y float64) float64 {
	return x * y
}

func div(x, y float64) float64 {
	log.Panicln("huh")
	return x / y
}

func main() {
	wg := sync.WaitGroup{}
	wg.Add(2)

	x, y := 10.0, 0.0

	go func() {
		defer wg.Done()
		fmt.Println(mul(x, y))
	}()
	go func() {
		defer wg.Done()
		fmt.Println(div(x, y))
	}()
	wg.Wait()

	fmt.Println("Entro? o:")
}