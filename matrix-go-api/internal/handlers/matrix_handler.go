package handlers

import (
	"matrix-go-api/internal/models"
	"matrix-go-api/internal/services"

	"github.com/gofiber/fiber/v2"
)

type MatrixHandler struct {
	service services.IMatrixService
}

func NewMatrixHandler(service services.IMatrixService) *MatrixHandler {
	return &MatrixHandler{service: service}
}

func (h *MatrixHandler) ProcessMatrix(c *fiber.Ctx) error {
	var req models.MatrixRequest

	if err := c.BodyParser(&req); err != nil {
		return c.Status(fiber.StatusBadRequest).JSON(fiber.Map{
			"error": "Error al procesar el JSON de entrada",
		})
	}

	if len(req.Data) == 0 {
		return c.Status(fiber.StatusBadRequest).JSON(fiber.Map{
			"error": "La matriz no puede estar vacía",
		})
	}

	rotated := h.service.Rotate90(req.Data)

	q, r, err := h.service.FactorizeQR(req.Data)
	if err != nil {
		return c.Status(fiber.StatusInternalServerError).JSON(fiber.Map{
			"error": "Error al calcular la factorización QR",
		})
	}

	response := models.MatrixResponse{
		Original: req.Data,
		Rotated:  rotated,
		Q:        q,
		R:        r,
	}

	return c.Status(fiber.StatusOK).JSON(response)
}