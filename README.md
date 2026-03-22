# CEPLAN - Gestión de Usuarios

![Project Status](https://img.shields.io/badge/Status-In%20Development-yellow?style=for-the-badge)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![Type](https://img.shields.io/badge/Type-Technical%20Assessment-blue?style=for-the-badge)

Solución integral para la administración de identidades y perfiles, diseñada bajo una arquitectura escalable y centrada en la experiencia de usuario (UX). Este proyecto implementa flujos de autenticación robustos y un sistema de monitoreo de sesiones en tiempo real.

## 🛠 Stack Tecnológico

* **Backend:** ASP.NET Core 9.0 (MVC)
* **Data:** Entity Framework Core + SQL Server
* **Frontend:** Razor Pages, Bootstrap 5, Vanilla JS (ES6+)
* **UI/UX:** Design Tokens (N2 Neutral Scale, Body B1 Typography)
* **Security:** Cookie Authentication & HMACSHA512 Password Hashing

---

## 🚀 Características Principales

### Autenticación y Seguridad
Implementación de un ciclo de vida de usuario seguro mediante `AccountController`. El sistema incluye:
* **Protección ante Fuerza Bruta:** Bloqueo automático de cuenta tras 5 intentos fallidos.
* **Esquemas de Identidad:** Persistencia basada en Cookies con protección Anti-Forgery.

### Control de Inactividad (Session Management)
Motor de monitoreo en el cliente que trackea eventos de usuario (clics, scroll, teclado). Al detectar inactividad:
* Sincroniza con el `ExpireTimeSpan` configurado en el servidor.
* Despliega un modal de advertencia con un contador regresivo de 60 segundos.
* Permite la extensión de sesión mediante un refresh asíncrono del token.

### Interfaz Adaptativa
Diseño responsivo optimizado para entornos corporativos, utilizando una paleta de colores institucional definida mediante variables CSS y componentes modulares (Sidebar colapsable, Grillas dinámicas).

---

## 📂 Arquitectura

El proyecto emplea el patrón **MVC** con una clara separación de responsabilidades:

* **Controllers:** Orquestación de lógica de negocio y flujos de autorización.
* **Models:** Entidades de dominio y ViewModels especializados para la capa de presentación.
* **Services:** Abstracción de servicios transversales (SMTP Email Service).
* **Data:** Capa de persistencia (`ApplicationDbContext`) y `DbSeeder` para automatización.
* **Migrations:** Historial de versiones de la base de datos (incluido para control de versiones).

---

## ⚙️ Instalación

1.  **Clonación del Repositorio:**
    ```bash
    git clone https://github.com/JhosepAC/Upscale.Web.git
    ```

2.  **Configuración de Base de Datos:**
    Ajustar el `ConnectionString` en `appsettings.json` con sus credenciales de SQL Server.

3.  **Despliegue de Esquema:**
    ```bash
    dotnet ef database update
    ```

4.  **Ejecución:**
    ```bash
    dotnet run
    ```

---

## 👥 Usuarios de prueba

| N.º Documento | Contraseña   | Nombre              | Cargo                     |
| ------------- | ------------ | ------------------- | ------------------------- |
| **71234567**  | `Admin@2026` | July Vargas Mendoza | Analista de Planeamiento  |
| **45678901**  | `User.9876`  | Ricardo Luna Perez  | Especialista Presupuestal |
| **12345678**  | `Clave#2024` | Fátima Encarnación  | Coordinadora RRHH         |


## 📩 Contact & Socials

[![Portfolio](https://img.shields.io/badge/Portfolio-Check%20it%20out-black?style=for-the-badge&logo=googlechrome&logoColor=white)](https://jhosep-ac.pages.dev)
[![Instagram](https://img.shields.io/badge/Instagram-Follow%20Me-E4405F?style=for-the-badge&logo=instagram&logoColor=white)](https://instagram.com/jh_slin)
[![Gmail](https://img.shields.io/badge/Gmail-Contact%20Me-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:jhosepjamil@gmail.com)
[![WhatsApp](https://img.shields.io/badge/WhatsApp-WRITE%20TO%20ME-25D366?style=for-the-badge&logo=whatsapp&logoColor=white)](https://wa.me/51978777386)