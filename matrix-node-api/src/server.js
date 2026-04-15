const express = require('express');
const cors = require('cors');
const statsRoutes = require('./routes/statsRoutes');

const app = express();
const PORT = process.env.PORT || 4000;

app.use(cors());
app.use(express.json()); 

app.use((req, res, next) => {
    console.log(`[${new Date().toISOString()}] ${req.method} ${req.url}`);
    next();
});

app.use('/api/stats', statsRoutes);

app.use((req, res) => {
    res.status(404).json({ success: false, error: 'Endpoint no encontrado' });
});

app.listen(PORT, () => {
    console.log(`Servidor Node.js (Estadísticas) iniciado en el puerto ${PORT}`);
});