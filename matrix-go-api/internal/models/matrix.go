package models

type MatrixRequest struct {
	Data [][]float64 `json:"data"`
}

type MatrixResponse struct {
	Original [][]float64 `json:"original"`
	Rotated  [][]float64 `json:"rotated"`
	Q        [][]float64 `json:"q_matrix"`
	R        [][]float64 `json:"r_matrix"`
}