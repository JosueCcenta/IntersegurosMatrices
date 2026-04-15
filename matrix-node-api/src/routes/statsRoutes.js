const express = require('express');
const router = express.Router();
const statsController = require('../controllers/statsController');

router.post('/calculate', statsController.processStats);

module.exports = router;