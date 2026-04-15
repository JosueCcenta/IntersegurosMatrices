<script setup>
import { ref, watch } from 'vue'
import axios from 'axios'

const token = ref('')
const rows = ref(3)
const cols = ref(3)
const matrix = ref([[1, 2, 3], [4, 5, 6], [7, 8, 9]]) 
const loading = ref(false)
const error = ref('')
const results = ref(null)

const gatewayUrl = import.meta.env.VITE_API_GATEWAY_URL;

const login = async () => {
  error.value = ''
  loading.value = true
  try {
    const res = await axios.post(`${gatewayUrl}/auth/login`)
    token.value = res.data.token
  } catch (err) {
    error.value = err
  } finally {
    loading.value = false
  }
}

const updateMatrix = () => {
  const newMatrix = []
  for (let i = 0; i < rows.value; i++) {
    const newRow = []
    for (let j = 0; j < cols.value; j++) {
      newRow.push(matrix.value[i]?.[j] || 0)
    }
    newMatrix.push(newRow)
  }
  matrix.value = newMatrix
}

watch([rows, cols], updateMatrix)

const analyzeMatrix = async () => {
  error.value = ''
  results.value = null
  loading.value = true
  
  const numericMatrix = matrix.value.map(row => row.map(val => Number(val)))

  try {
    const res = await axios.post(`${gatewayUrl}/analyze`, 
      { data: numericMatrix },
      { headers: { Authorization: `Bearer ${token.value}` } }
    )
    results.value = res.data
  } catch (err) {
    if (err.response?.status === 401) {
      error.value = 'No autorizado. Por favor, inicia sesión primero.'
    } else {
      error.value = err.response?.data || 'Error al procesar la matriz en los microservicios.'
    }
  } finally {
    loading.value = false
  }
}

const formatNum = (num) => Number(num.toFixed(4))
</script>

<template>
  <main class="min-h-screen py-10 px-4 md:px-10 max-w-6xl mx-auto font-sans">
    
    <header class="mb-10 text-center">
      <h1 class="text-4xl font-extrabold text-blue-900 tracking-tight">Interseguro <span class="text-blue-600">Matrix Engine</span></h1>
      <p class="text-gray-500 mt-2">Arquitectura de Microservicios: .NET 10 Gateway ➔ Go ➔ Node.js</p>
    </header>

    <div v-if="error" class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-6 text-center shadow-sm">
      {{ error }}
    </div>

    <section v-if="!token" class="bg-white rounded-xl shadow-md p-8 text-center max-w-md mx-auto border border-gray-100">
      <div class="mb-4 bg-blue-50 w-16 h-16 rounded-full flex items-center justify-center mx-auto text-blue-500">
        <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"></path></svg>
      </div>
      <h2 class="text-xl font-bold mb-2">Acceso Restringido</h2>
      <p class="text-gray-500 mb-6 text-sm">El API Gateway requiere un token JWT para operar.</p>
      <button @click="login" :disabled="loading" class="w-full bg-blue-600 hover:bg-blue-700 text-white font-bold py-3 px-4 rounded transition-colors disabled:opacity-50">
        {{ loading ? 'Autenticando...' : 'Generar JWT de Acceso' }}
      </button>
    </section>

    <div v-else class="grid grid-cols-1 lg:grid-cols-12 gap-8">
      
      <section class="lg:col-span-4 bg-white rounded-xl shadow-md p-6 border border-gray-100 self-start">
        <div class="flex items-center justify-between mb-6">
          <h2 class="text-lg font-bold text-gray-800">Constructor de Matriz</h2>
          <span class="bg-green-100 text-green-800 text-xs px-2 py-1 rounded-full font-semibold border border-green-200">JWT Activo</span>
        </div>

        <div class="flex space-x-4 mb-6">
          <div class="w-1/2">
            <label class="block text-sm font-medium text-gray-700 mb-1">Filas (n)</label>
            <input type="number" v-model="rows" min="1" max="10" class="w-full border-gray-300 rounded-md shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 transition px-3 py-2 bg-gray-50 border">
          </div>
          <div class="w-1/2">
            <label class="block text-sm font-medium text-gray-700 mb-1">Columnas (m)</label>
            <input type="number" v-model="cols" min="1" max="10" class="w-full border-gray-300 rounded-md shadow-sm focus:border-blue-500 focus:ring focus:ring-blue-200 transition px-3 py-2 bg-gray-50 border">
          </div>
        </div>

        <div class="overflow-x-auto mb-6 bg-gray-50 p-4 rounded-lg border border-gray-200">
          <div v-for="(row, rIndex) in matrix" :key="rIndex" class="flex mb-2 space-x-2 last:mb-0 justify-center">
            <input 
              v-for="(col, cIndex) in row" 
              :key="cIndex" 
              v-model.number="matrix[rIndex][cIndex]"
              type="number"
              class="w-14 h-12 text-center text-lg font-semibold text-blue-900 border-gray-300 rounded shadow-sm focus:border-blue-500 focus:ring-blue-200 border"
            >
          </div>
        </div>

        <button @click="analyzeMatrix" :disabled="loading" class="w-full bg-indigo-600 hover:bg-indigo-700 text-white font-bold py-3 px-4 rounded-lg transition shadow flex justify-center items-center">
          <span v-if="loading">Procesando en Microservicios...</span>
          <span v-else>Analizar Matriz</span>
        </button>
      </section>

      <section class="lg:col-span-8 space-y-6">
        <div v-if="!results && !loading" class="bg-white rounded-xl shadow-md p-12 text-center border border-gray-100 flex flex-col items-center justify-center h-full text-gray-400">
           <svg class="w-16 h-16 mb-4 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9 17V7m0 10a2 2 0 01-2 2H5a2 2 0 01-2-2V7a2 2 0 012-2h2a2 2 0 012 2m0 10a2 2 0 002 2h2a2 2 0 002-2M9 7a2 2 0 012-2h2a2 2 0 012 2m0 10V7m0 10a2 2 0 002 2h2a2 2 0 002-2V7a2 2 0 00-2-2h-2a2 2 0 00-2 2"></path></svg>
           <p class="text-lg">Ingresa los datos y haz clic en analizar.</p>
        </div>

        <div v-if="loading" class="bg-white rounded-xl shadow-md p-12 text-center border border-gray-100 flex flex-col items-center justify-center h-full">
           <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600 mb-4"></div>
           <p class="text-indigo-600 font-semibold animate-pulse">Orquestando llamadas entre Go y Node.js...</p>
        </div>

        <template v-if="results">
          <div class="grid grid-cols-2 md:grid-cols-5 gap-4">
            <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-100 text-center">
              <p class="text-xs text-gray-500 uppercase tracking-wider font-bold mb-1">Máximo</p>
              <p class="text-2xl font-black text-indigo-600">{{ results.statistics.max }}</p>
            </div>
            <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-100 text-center">
              <p class="text-xs text-gray-500 uppercase tracking-wider font-bold mb-1">Mínimo</p>
              <p class="text-2xl font-black text-indigo-600">{{ results.statistics.min }}</p>
            </div>
            <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-100 text-center">
              <p class="text-xs text-gray-500 uppercase tracking-wider font-bold mb-1">Promedio</p>
              <p class="text-2xl font-black text-indigo-600">{{ formatNum(results.statistics.average) }}</p>
            </div>
            <div class="bg-white p-4 rounded-xl shadow-sm border border-gray-100 text-center">
              <p class="text-xs text-gray-500 uppercase tracking-wider font-bold mb-1">Suma</p>
              <p class="text-2xl font-black text-indigo-600">{{ results.statistics.sum }}</p>
            </div>
            <div :class="results.statistics.isDiagonal ? 'bg-green-50 border-green-200' : 'bg-white border-gray-100'" class="p-4 rounded-xl shadow-sm border text-center">
              <p class="text-xs text-gray-500 uppercase tracking-wider font-bold mb-1">¿Diagonal?</p>
              <p :class="results.statistics.isDiagonal ? 'text-green-600' : 'text-gray-400'" class="text-2xl font-black">
                {{ results.statistics.isDiagonal ? 'SÍ' : 'NO' }}
              </p>
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-5">
              <h3 class="text-sm font-bold text-gray-400 uppercase tracking-wider mb-4 border-b pb-2">Matriz Rotada (90°)</h3>
              <div class="overflow-x-auto">
                <table class="w-full">
                  <tr v-for="(row, i) in results.mathResults.rotated" :key="'rot'+i">
                    <td v-for="(val, j) in row" :key="'rot'+i+j" class="text-center p-2 border-b border-gray-50 font-mono text-gray-700">
                      {{ val }}
                    </td>
                  </tr>
                </table>
              </div>
            </div>

            <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-5">
              <h3 class="text-sm font-bold text-gray-400 uppercase tracking-wider mb-4 border-b pb-2">Matriz R (Factorización QR)</h3>
              <div class="overflow-x-auto">
                <table class="w-full">
                  <tr v-for="(row, i) in results.mathResults.r_Matrix" :key="'r'+i">
                    <td v-for="(val, j) in row" :key="'r'+i+j" class="text-center p-2 border-b border-gray-50 font-mono text-sm text-gray-600">
                      {{ formatNum(val) }}
                    </td>
                  </tr>
                </table>
              </div>
            </div>
          </div>

        </template>
      </section>
    </div>
  </main>
</template>