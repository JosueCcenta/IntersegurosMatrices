const statsService = require('../services/statsService');

class StatsController {
    processStats(req, res) {
        try {
            const matrixToAnalyze = req.body.rotated || req.body.data;

            if (!matrixToAnalyze) {
                return res.status(400).json({ 
                    success: false, 
                    error: "No se proporcionó una matriz válida en el body (se espera 'rotated' o 'data')" 
                });
            }

            const stats = statsService.calculateStats(matrixToAnalyze);

            return res.status(200).json({
                success: true,
                stats: stats
            });

        } catch (error) {
            return res.status(500).json({
                success: false,
                error: error.message
            });
        }
    }
}

module.exports = new StatsController();