#Matrix Orchestrator

Esta solución consiste en una arquitectura de microservicios políglota diseñada para el procesamiento dinámico y análisis estadístico de matrices cuadradas. El sistema demuestra la integración de diversos ecosistemas tecnológicos coordinados a través de un patrón de **API Gateway** centralizado.

---

## Resumen Arquitectónico

El sistema se divide en cuatro capas de responsabilidad segregada, optimizadas para la escalabilidad independiente y la portabilidad mediante contenedores:

* **Core Orchestrator (.NET 10):** Actúa como el API Gateway único. Gestiona la seguridad (JWT), la validación de contratos y la orquestación de llamadas paralelas/secuenciales hacia los motores de cómputo.
* **Mathematical Engine (Go):** Servicio de alto rendimiento especializado en operaciones de álgebra lineal. Implementa algoritmos de rotación de matrices y factorización QR utilizando la librería especializada `gonum`.
* **Statistical Processor (Node.js):** Encargado de la agregación de datos y cálculo de métricas sobre los resultados procesados, aprovechando la eficiencia de Node para tareas de I/O intensivo.
* **Frontend (Vue 3):** Interfaz reactiva desarrollada con Vite y Tailwind CSS que garantiza una experiencia de usuario fluida y validaciones de datos en tiempo real (Client-side validation).



---

## Stack Tecnológico

| Componente | Tecnología | Justificación Técnica |
| :--- | :--- | :--- |
| **Orquestador** | ASP.NET Core 10 | Resiliencia mediante `HttpClientFactory` y seguridad robusta. |
| **Cómputo** | Go | Eficiencia en runtime y manejo óptimo de memoria para cálculos pesados. |
| **Estadísticas** | Node.js | Agilidad en el procesamiento de estructuras JSON dinámicas. |
| **Infraestructura** | Docker & GCP | Despliegue serverless (Cloud Run) para escalabilidad horizontal automática. |

---

## Configuración del Entorno

El proyecto sigue los principios de **Twelve-Factor App**, utilizando variables de entorno para la configuración de endpoints. Esto permite que la misma imagen de contenedor sea promovida a través de diferentes entornos (Desarrollo, QA, Producción) sin cambios en el código.

### Variables Clave
* `GATEWAY_JWT_SECRET`: Clave de cifrado para la validación de tokens.
* `SERVICE_GO_URL`: Endpoint interno para el procesamiento matemático.
* `SERVICE_NODE_URL`: Endpoint interno para el procesamiento estadístico.
* `VITE_API_GATEWAY_URL`: Endpoint público expuesto por el orquestador.

---

## Decisiones de Diseño Significativas

### 1. Manejo Dinámico de Dimensiones
Se implementó el uso de **Slices** en el motor de Go en lugar de arreglos de tamaño fijo. Esto permite que el sistema sea agnóstico a la dimensión de la matriz de entrada ($n \times n$), gestionando la memoria de forma dinámica en tiempo de ejecución.

### 2. Propagación de Errores (Error Tracing)
El Gateway implementa una lógica de captura de excepciones que estandariza los errores provenientes de los microservicios internos. Se asegura que el cliente reciba un código de estado HTTP adecuado y un mensaje descriptivo, abstrayendo la complejidad de la comunicación entre servicios.

### 3. Seguridad Centralizada
Toda la lógica de autenticación reside en el Gateway. Los microservicios internos están diseñados bajo una arquitectura de "confianza cero", delegando la validación del JWT exclusivamente al orquestador para reducir la superficie de ataque.

---

## Despliegue e Infraestructura

Cada servicio cuenta con un **Dockerfile** multi-etapa (*multi-stage build*), lo que garantiza imágenes ligeras y seguras al separar el entorno de compilación del entorno de ejecución final.
