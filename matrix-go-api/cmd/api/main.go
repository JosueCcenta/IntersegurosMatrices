package main

import (
	"log"
	"matrix-go-api/internal/handlers"
	"matrix-go-api/internal/services"

	"github.com/gofiber/fiber/v2"
	"github.com/gofiber/fiber/v2/middleware/logger"
	"github.com/gofiber/fiber/v2/middleware/recover"
)

func main() {
	app := fiber.New()

	app.Use(logger.New())
	app.Use(recover.New())

	matrixService := services.NewMatrixService()
	matrixHandler := handlers.NewMatrixHandler(matrixService)

	api := app.Group("/api")
	api.Post("/process", matrixHandler.ProcessMatrix)

	log.Println("Servidor Go iniciado en el puerto 3000")
	if err := app.Listen(":3000"); err != nil {
		log.Fatalf("Error al iniciar el servidor: %v", err)
	}
}
