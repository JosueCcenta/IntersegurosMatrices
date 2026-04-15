package services

import (
	"errors"
	"gonum.org/v1/gonum/mat"
)

type IMatrixService interface {
	Rotate90(matrix [][]float64) [][]float64
	FactorizeQR(matrix [][]float64) ([][]float64, [][]float64, error)
}

type MatrixService struct{}

func NewMatrixService() IMatrixService {
	return &MatrixService{}
}

func (s *MatrixService) Rotate90(matrix [][]float64) [][]float64 {
	if len(matrix) == 0 || len(matrix[0]) == 0 {
		return matrix
	}

	rows := len(matrix)
	cols := len(matrix[0])
	rotated := make([][]float64, cols)

	for i := 0; i < cols; i++ {
		rotated[i] = make([]float64, rows)
		for j := 0; j < rows; j++ {
			rotated[i][j] = matrix[rows-1-j][i]
		}
	}
	return rotated
}

func (s *MatrixService) FactorizeQR(input [][]float64) ([][]float64, [][]float64, error) {
	rows := len(input)
	if rows == 0 {
		return nil, nil, errors.New("matriz vacía")
	}
	cols := len(input[0])

	data := make([]float64, 0, rows*cols)
	for _, row := range input {
		data = append(data, row...)
	}
	denseMat := mat.NewDense(rows, cols, data)

	var qr mat.QR
	qr.Factorize(denseMat)

	var q, r mat.Dense
	qr.QTo(&q)
	qr.RTo(&r)

	return convertToSlices(&q), convertToSlices(&r), nil
}

func convertToSlices(m *mat.Dense) [][]float64 {
	r, c := m.Dims()
	res := make([][]float64, r)
	for i := 0; i < r; i++ {
		res[i] = make([]float64, c)
		for j := 0; j < c; j++ {
			res[i][j] = m.At(i, j)
		}
	}
	return res
}