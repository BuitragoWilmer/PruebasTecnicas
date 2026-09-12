   
CREATE DATABASE TaskManagementDB;

USE TaskManagementDB;

CREATE TABLE Users (
    UserId     INT IDENTITY(1,1) PRIMARY KEY,
    FullName   NVARCHAR(150)     NOT NULL,
    Email      NVARCHAR(200)     NOT NULL UNIQUE,
    CreatedAt  DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE TaskStatuses (
    StatusId    TINYINT      PRIMARY KEY,
    StatusName  VARCHAR(20)  NOT NULL UNIQUE
);

CREATE TABLE TaskItems (
    TaskId          INT IDENTITY(1,1) PRIMARY KEY,
    Title           NVARCHAR(200)     NOT NULL,
    Description     NVARCHAR(1000)    NULL,
    AssignedUserId  INT                NOT NULL,
    StatusId        TINYINT            NOT NULL DEFAULT (1),
    AdditionalInfo  NVARCHAR(MAX)      NULL,   -- el JSON completo
    CreatedAt       DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt       DATETIME2          NULL,

    Priority AS CAST(JSON_VALUE(AdditionalInfo, '$.priority') AS VARCHAR(10)) PERSISTED,

    DueDate AS CONVERT(DATE, JSON_VALUE(AdditionalInfo, '$.dueDate'), 126) PERSISTED,

    CONSTRAINT FK_Tasks_Users FOREIGN KEY (AssignedUserId)
        REFERENCES Users (UserId),
    CONSTRAINT FK_Tasks_TaskStatuses FOREIGN KEY (StatusId)
        REFERENCES TaskStatuses (StatusId),
    CONSTRAINT CHK_Tasks_AdditionalInfo_IsJSON
        CHECK (AdditionalInfo IS NULL OR ISJSON(AdditionalInfo) = 1)
);


CREATE INDEX IX_Tasks_Priority ON Tasks (Priority);
CREATE INDEX IX_Tasks_DueDate  ON Tasks (DueDate);

-- Tareas por usuario, ya ordenadas por fecha
CREATE INDEX IX_Tasks_AssignedUserId_CreatedAt
    ON TaskItems (AssignedUserId, CreatedAt DESC)
    INCLUDE (Title, StatusId);

-- Filtro por estado, ordenado por fecha
CREATE INDEX IX_Tasks_StatusId_CreatedAt
    ON TaskItems (StatusId, CreatedAt DESC)
    INCLUDE (Title, AssignedUserId);

INSERT INTO TaskStatuses (StatusId, StatusName) VALUES
    (1, 'Pending'),
    (2, 'InProgress'),
    (3, 'Done');

INSERT INTO Users (FullName, Email) VALUES
    ('Ana Torres', 'ana.torres@empresa.com'),
    ('Luis Perez',  'luis.perez@empresa.com');

INSERT INTO TaskItems (Title, Description, AssignedUserId, StatusId, AdditionalInfo, CreatedAt)
VALUES
(
    'Diseñar wireframes',
    'Wireframes de la pantalla de login',
    1,
    1, 
    N'{"priority":"High","dueDate":"2026-09-20","tags":["ui","design"],"estimatedHours":8}',
    GETDATE()
),
(
    'Configurar CI/CD',
    'Pipeline de build y deploy',
    2,
    2,
    N'{"priority":"Medium","dueDate":"2026-09-25","tags":["devops"],"estimatedHours":5}',
    GETDATE()
);


--CONSULTAS ADICIONALES 

-- Obtener tareas por usuario, ordenadas por fecha de creación
SELECT t.TaskId, t.Title, s.StatusName, t.CreatedAt
FROM TaskItems t
JOIN TaskStatuses s ON s.StatusId = t.StatusId
WHERE t.AssignedUserId = 1
ORDER BY t.CreatedAt DESC;

 
-- Filtrar tareas por estado
SELECT t.TaskId, t.Title, u.FullName, t.CreatedAt
FROM TaskItems t
JOIN Users u        ON u.UserId  = t.AssignedUserId
JOIN TaskStatuses s ON s.StatusId = t.StatusId
WHERE s.StatusName = 'Pending'
ORDER BY t.CreatedAt DESC;


--CONSULTAS ADICIONALES JSON
 

-- Validar que el JSON almacenado es válido
SELECT TaskId, Title, ISJSON(AdditionalInfo) AS EsJsonValido
FROM TaskItems;

-- Leer un valor escalar del JSON
SELECT TaskId, Title,
       JSON_VALUE(AdditionalInfo, '$.priority') AS Priority,
       JSON_VALUE(AdditionalInfo, '$.dueDate')  AS DueDate
FROM TaskItems;


-- Leer un fragmento/array del JSON
SELECT TaskId, Title, JSON_QUERY(AdditionalInfo, '$.tags') AS Tags
FROM TaskItems;

-- Filtrar usando un valor dentro del JSON
SELECT TaskId, Title
FROM TaskItems
WHERE JSON_VALUE(AdditionalInfo, '$.priority') = 'High';



-- Actualizar un campo específico dentro del JSON
UPDATE TaskItems
SET AdditionalInfo = JSON_MODIFY(AdditionalInfo, '$.priority', 'Low')
WHERE TaskId = 1;




