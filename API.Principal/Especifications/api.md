# Especificaciones API 

Version: 1.0

Base Path:

/api/v1

---

# Reglas Generales 

Todas las APIs deben:

- Usar HTTPS.
- Usar JSON.
- Devolver códigos de estado HTTP apropiados.
- Validar las solicitudes entrantes.
- No exponer excepciones internas.
- No exponer entidades de dominio directamente.
- Devolver DTOs.
- Hacer cumplir las políticas de autorización.
- Usar identificadores de correlación para la trazabilidad cuando estén disponibles.

---

# Autenticación

## API-AUTH-001

Caso de Uso:
UC-001 Registrar Usuario

Endpoint:

POST /api/v1/auth/register

Autenticación:

Anónimo

Solicitud:
{
  "nombre": "string",
  "email": "string",
  "password": "string"
}

Respuesta Exitosa:

201 Created

{
  "id": "uuid",
  "nombre": "string",
  "email": "string"
}

Posibles Errores:

400 Solicitud inválida
409 Email ya registrado

---

## API-AUTH-002

Caso de Uso:

UC-002 Iniciar Sesión	

Endpoint:

POST /api/v1/auth/login

Authentication:

Anonymous

Request:

{
  "email": "string",
  "password": "string"
}

Success:

200 OK

{
  "accessToken": "string",
  "expiresIn": 3600
}

Possible Errors:

400 Invalid request
401 Invalid credentials

---

# Clientes  y Vehiculos

# Courses

## API-Clientes-001

Caso de Uso:

UC-CV-01 Crear Cliente

Endpoint:

POST /api/v1/clientes

Authorization:

Policy: CanManageClientes

Request:

{
  "title": "string",
  "description": "string",
  "level": "string"
}

Success:

201 Created

{
  "id": "uuid",
  "title": "string",
  "status": "Draft"
}

Errors:

400 Invalid request
401 Unauthenticated
403 Unauthorized

---

## API-Clientes-002

Caso de uso:

UC-CV-03 Actualizar Cliente

Endpoint:

PUT /api/v1/clientes/{clienteId}
Authorization:

Policy: CanManageClientes

---

## API-Clientes-003

Caso de uso:

UC-CV-02 Consultar Cliente

Endpoint:
GET /api/v1/clientes/{clienteId}
GET /api/v1/clientes/{nombre}

--
## API-Desactivar-Cliente

Caso de Uso:

UC-CV-04 Desactivar Cliente

Endpoint:
DELETE /api/v1/clientes/{clienteId}

Authorization:

Policy: CanManageClientes

Errors:

400 Invalid request
401 Unauthenticated
403 Unauthorized



## API-COURSE-003

Use Case:

UC-006 Publish Course

Endpoint:

POST /api/v1/courses/{courseId}/publish

Authorization:

Policy: CanPublishCourse

Success:

200 OK

Possible Errors:

400 Course does not satisfy publication rules
404 Course not found
403 User cannot publish this course

---

## API-VALIDAR-CLIENTE

Use Case:

UC-006 Validar Cliente

Endpoint:

GET /api/v1/clientes/validar/publish

Authorization:

Policy: CanPublishCourse

Success:

200 OK

Possible Errors:

400 Course does not satisfy publication rules
404 Course not found
403 User cannot publish this course




---
## API-COURSE-004

Endpoint:

GET /api/v1/courses/{courseId}

Authorization:

Authenticated

Success:

200 OK

---




# Enrollment

## API-ENROLLMENT-001

Use Case:

UC-009 Enroll Student

Endpoint:

POST /api/v1/courses/{courseId}/enroll

Authorization:

Policy: Student

Success:

201 Created

Possible Errors:

400 Course unavailable
409 Student already enrolled
404 Course not found

---

# Progress

## API-PROGRESS-001

Use Case:

UC-010 Update Progress

Endpoint:

PUT /api/v1/courses/{courseId}/progress

Authorization:

Student