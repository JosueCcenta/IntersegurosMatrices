class StatsService {
    calculateStats(matrix) {
        if (!matrix || matrix.length === 0 || !Array.isArray(matrix[0])) {
            throw new Error("Matriz inválida o vacía");
        }

        const rows = matrix.length;
        const cols = matrix[0].length;
        
        let max = -Infinity;
        let min = Infinity;
        let sum = 0;
        let count = 0;
        let isDiagonal = true;

        for (let i = 0; i < rows; i++) {
            for (let j = 0; j < cols; j++) {
                const val = matrix[i][j];
                
                if (val > max) max = val;
                if (val < min) min = val;
                sum += val;
                count++;

                if (i !== j && val !== 0) {
                    isDiagonal = false;
                }
            }
        }

        if (rows !== cols) {
            isDiagonal = false;
        }

        const average = sum / count;

        return {
            max,
            min,
            average,
            sum,
            isDiagonal
        };
    }
}

module.exports = new StatsService();