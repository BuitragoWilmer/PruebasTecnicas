# Pasos para ejecutar el proyecto

## Requisitos previos

Antes de ejecutar el proyecto, asegúrate de tener instalado:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) versión 18.19+ o superior
- npm
- Angular CLI 19 (opcional, también se puede usar `npx`)
- Git


---

## 1. Clonar el repositorio

```bash
git clone <URL_DEL_REPOSITORIO>
cd <NOMBRE_DEL_REPOSITORIO>
```

---

## 2. Configuración de la base de datos

La API `Web.API` ya se encuentra configurada para conectarse a una base de datos SQL Server alojada en la nube.
La cadena de conexión utilizada es la siguiente:

```txt
Server=db67828.public.databaseasp.net; Database=db67828; User Id=db67828; Password=5c=N-Yj89_Bt; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True
```

Esta cadena ya está incluida en la configuración de la API, por lo que **no es necesario crear ni configurar una base de datos local**.

Si se desea modificar, se debe revisar el archivo `appsettings.json` del proyecto `Web.API`, en la sección `ConnectionStrings`.


---

## 3. Ejecutar la API (.NET 8)

La API está desarrollada en .NET 8 y el proyecto principal se llama `Web.API`.
Debe ejecutarse en el puerto `5014`, ya que el frontend está configurado para consumirla desde:

```txt
http://localhost:5014
```

Desde la raíz del repositorio, ejecutar:

```bash
dotnet run --project ./Web.API/Web.API.csproj --urls "http://localhost:5014"
```

También puedes ingresar directamente a la carpeta del proyecto y ejecutar:

```bash
cd Web.API
dotnet restore
dotnet run --urls "http://localhost:5014"
```



Verifica que en la consola aparezca algo similar a:

```txt
Now listening on: http://localhost:5014
```

La API tiene Swagger habilitado, puedes comprobar en:

```txt
http://localhost:5014/swagger
```

---

## 4. Ejecutar el frontend (Angular 19)

El frontend está desarrollado en Angular 19 y consume la API desde `http://localhost:5014`.

En otra terminal, desde la carpeta del frontend, ejecutar:

```bash
cd task-management-frontend
npm install
npm start
```

O bien:

```bash
npx ng serve --port 4200
```

Luego abre en el navegador:

```txt
http://localhost:4200
```

El frontend ya debería estar apuntando a la API en `http://localhost:5014`.
Si necesitas revisarlo, verifica el archivo de entorno correspondiente:

```txt
src/environments/environment.ts
```

---

## 5. Orden de ejecución

1. Iniciar la API en `http://localhost:5014`
2. Iniciar el frontend en `http://localhost:4200`
3. Abrir el navegador en `http://localhost:4200`

Ambos proyectos deben permanecer en ejecución al mismo tiempo.

---

## Notas adicionales


- Si el puerto `5014` está ocupado, se debe liberar o cambiar la URL tanto en la API como en el frontend.
- La base de datos es remota, por lo que se requiere conexión a Internet.
- No se necesita instalar SQL Server localmente.


# Decisiones técnicas

En esta sección se describen las decisiones de diseño y arquitectura tomadas durante el desarrollo del proyecto, así como la justificación de cada una.

---

## Backend — Arquitectura limpia con CQRS

Para la construcción de la API se optó por implementar **Clean Architecture** (Arquitectura Limpia) junto con el patrón **CQRS** (Command Query Responsibility Segregation).

Esta combinación permitió organizar el proyecto en capas bien definidas, cada una con una responsabilidad clara:

- **Domain:** entidades, reglas de negocio y contratos principales.
- **Application:** casos de uso, comandos, queries y sus respectivos handlers (CQRS).
- **Infrastructure:** acceso a datos, persistencia y servicios externos.
- **Web.API:** exposición de los endpoints mediante controladores.

### Motivación

Si bien la naturaleza y el alcance de esta prueba técnica **no exigen una infraestructura tan elaborada**, la decisión de aplicarla responde a los siguientes criterios:

1. **Evidenciar conocimientos** en patrones y arquitecturas utilizados en entornos profesionales.
2. **Segregación de responsabilidades**, separando las operaciones de lectura (*queries*) de las de escritura (*commands*), lo que facilita el mantenimiento y la escalabilidad.
3. **Bajo acoplamiento y alta cohesión**, permitiendo que cada capa evolucione de forma independiente.
4. **Facilidad de testing**, ya que los casos de uso quedan aislados de detalles de infraestructura.
5. **Consistencia con buenas prácticas**, alineadas a estándares comunes en equipos de desarrollo .NET.

Cabe aclarar que se trata de una implementación **intencionalmente demostrativa**: se buscó que la solución reflejara cómo se abordaría un proyecto real de mayor complejidad, sin perder de vista que el objetivo principal era cumplir con los requerimientos funcionales planteados.

---

## Frontend — Simplicidad y prioridad en la funcionalidad

En la parte del frontend se desarrolló una aplicación en **Angular 19** con un enfoque pragmático, priorizando la funcionalidad y la claridad sobre una estructura compleja.

### Decisiones tomadas

1. **Sin routing entre vistas:** dado el alcance del proyecto, no se implementó navegación entre rutas, ya que toda la interacción ocurre dentro de una misma vista o flujo.
2. **Prioridad en la funcionalidad:** se optó por garantizar que los flujos principales (consumo de la API, visualización y manipulación de datos) funcionaran correctamente antes que por refinar la separación en capas o componentes.

### Reconocimiento de mejoras posibles

Se es consciente de que **la separación de responsabilidades en el frontend pudo haber sido más estricta**, por ejemplo:

- Dividir la lógica en componentes más pequeños y reutilizables.
- Hacer uso de interfaces.
- Incorporar routing para preparar la aplicación ante un crecimiento futuro.
- Aplicar patrones o gestión de estado centralizada.

Estas mejoras no se implementaron porque **no eran necesarias para el alcance de la prueba**, y la prontitud de entrega del proyecto.

---

## Consideraciones generales

- Las decisiones anteriores reflejan un balance entre **evidenciar conocimiento técnico** (en el backend) y **priorizar la entrega funcional** (en el frontend).
- Ambas partes fueron pensadas para ser **fáciles de ejecutar y evaluar** por parte de quien revise la prueba técnica.
- La API se conecta a una base de datos remota ya configurada, por lo que no se requiere infraestructura adicional para su funcionamiento.

> Nota: Cabe aclarar que para el desarrollo de esta prueba se hizo uso de herramientas de Inteligencia Artificial como apoyo. Sin esta ayuda, cumplir con los tiempos de entrega habría sido considerablemente más complejo.
No obstante, el uso de IA se llevó a cabo de forma **responsable**, reconociendo sus límites y **supervisando cada línea escrita**. Todas las decisiones técnicas, la validación del código y la verificación del correcto funcionamiento fueron realizadas de manera consciente, asegurando que la solución final cumpla con los requerimientos solicitados.
