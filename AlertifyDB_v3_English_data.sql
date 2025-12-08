USE [AlertifyDB];
GO

-- Agregar columna MustChangePassword
IF COL_LENGTH('User', 'MustChangePassword') IS NULL
BEGIN
    ALTER TABLE [User]
    ADD MustChangePassword BIT NOT NULL DEFAULT(0);
END
GO

-- Main Admin User (ID = 1)
INSERT INTO [User] (Email, Password, FirstName, FirstLastName, SecondLastName, NationalID, Phone, Role, LastAccess, CreatedBy, CreationDate, Status, MustChangePassword)
VALUES 
('admin@alertify.com', '$2a$11$JApuA5fKPGea5TWk4fp2NemWS9Sjuieoheb3Tr8JFS6soJSY4Ll6C', 'Carlos', 'Rodriguez', 'Mamani', '5678901', '78945612', 'Admin', GETDATE(), 1, GETDATE(), 'Activo', 0);

-- Additional Admins
INSERT INTO [User] (Email, Password, FirstName, FirstLastName, SecondLastName, NationalID, Phone, Role, LastAccess, CreatedBy, CreationDate, Status, MustChangePassword)
VALUES 
('admin.operations@alertify.com', '$2a$11$JApuA5fKPGea5TWk4fp2NemWS9Sjuieoheb3Tr8JFS6soJSY4Ll6C', 'Maria', 'Gutierrez', 'Flores', '6789012', '79856234', 'Admin', GETDATE(), 1, GETDATE(), 'Activo', 0),
('admin.supervisor@alertify.com', '$2a$11$JApuA5fKPGea5TWk4fp2NemWS9Sjuieoheb3Tr8JFS6soJSY4Ll6C', 'Jorge', 'Mendoza', 'Vargas', '7890123', '70123456', 'Admin', DATEADD(HOUR, -2, GETDATE()), 1, GETDATE(), 'Activo', 0);

-- Verified Citizens
INSERT INTO [User] (Email, Password, FirstName, FirstLastName, SecondLastName, NationalID, Phone, Role, LastAccess, ProfilePhotoURL, CreatedBy, CreationDate, Status, MustChangePassword)
VALUES 
('juan.perez@gmail.com', '$2a$11$Qs5oav2FaHcgI6rXehpb8Oa1iFKgEfkLbJkaokw1RgBw3CnVz6Uc.', 'Juan', 'Perez', 'Lopez', '1234567', '71234567', 'Ciudadano', DATEADD(MINUTE, -30, GETDATE()), '/uploads/profiles/juan_perez.jpg', 1, DATEADD(DAY, -15, GETDATE()), 'Activo', 0),
('maria.gonzalez@hotmail.com', '$2a$11$Qs5oav2FaHcgI6rXehpb8Oa1iFKgEfkLbJkaokw1RgBw3CnVz6Uc.', 'Maria', 'Gonzalez', 'Torrez', '2345678', '72345678', 'Ciudadano', DATEADD(HOUR, -1, GETDATE()), NULL, 1, DATEADD(DAY, -20, GETDATE()), 'Activo', 0),
('pedro.martinez@yahoo.com', '$2a$11$Qs5oav2FaHcgI6rXehpb8Oa1iFKgEfkLbJkaokw1RgBw3CnVz6Uc.', 'Pedro', 'Martinez', 'Rojas', '3456789', '73456789', 'Ciudadano', DATEADD(DAY, -2, GETDATE()), NULL, 1, DATEADD(DAY, -10, GETDATE()), 'Activo', 0),
('ana.silva@gmail.com', '$2a$11$Qs5oav2FaHcgI6rXehpb8Oa1iFKgEfkLbJkaokw1RgBw3CnVz6Uc.', 'Ana', 'Silva', 'Morales', '4567890', '74567890', 'Ciudadano', GETDATE(), '/uploads/profiles/ana_silva.jpg', 1, DATEADD(DAY, -5, GETDATE()), 'Activo', 0),
('luis.torrez@outlook.com', '$2a$11$Qs5oav2FaHcgI6rXehpb8Oa1iFKgEfkLbJkaokw1RgBw3CnVz6Uc.', 'Luis', 'Torrez', 'Castro', '5678901-CB', '75678901', 'Ciudadano', DATEADD(HOUR, -5, GETDATE()), NULL, 1, DATEADD(DAY, -30, GETDATE()), 'Activo', 0);

-- Citizens pending verification
INSERT INTO [User] (Email, Password, FirstName, FirstLastName, SecondLastName, NationalID, Phone, Role, CreatedBy, CreationDate, Status, MustChangePassword)
VALUES 
('carlos.ramos@gmail.com', '$2a$11$Qs5oav2FaHcgI6rXehpb8Oa1iFKgEfkLbJkaokw1RgBw3CnVz6Uc.', 'Carlos', 'Ramos', 'Quispe', '6789012', '76789012', 'Ciudadano', 1, DATEADD(HOUR, -4, GETDATE()), 'Activo', 0),
('sofia.mendez@hotmail.com', '$2a$11$Qs5oav2FaHcgI6rXehpb8Oa1iFKgEfkLbJkaokw1RgBw3CnVz6Uc.', 'Sofia', 'Mendez', 'Zapata', '7890123', '77890123', 'Ciudadano', 1, DATEADD(HOUR, -2, GETDATE()), 'Activo', 0);

-- Citizen with password reset token
INSERT INTO [User] (Email, Password, FirstName, FirstLastName, SecondLastName, NationalID, Phone, Role, LastAccess, CreatedBy, CreationDate, Status, MustChangePassword)
VALUES 
('roberto.flores@gmail.com', '$2a$11$Qs5oav2FaHcgI6rXehpb8Oa1iFKgEfkLbJkaokw1RgBw3CnVz6Uc.', 'Roberto', 'Flores', 'Condori', '8901234', '78901234', 'Ciudadano', DATEADD(DAY, -1, GETDATE()), 1, DATEADD(DAY, -45, GETDATE()), 'Activo', 0);

-- InActivo citizens
INSERT INTO [User] (Email, Password, FirstName, FirstLastName, SecondLastName, NationalID, Phone, Role, LastAccess, CreatedBy, CreationDate, ModifiedBy, ModificationDate, Status, MustChangePassword)
VALUES 
('inActivo.user@gmail.com', '$2a$11$Qs5oav2FaHcgI6rXehpb8Oa1iFKgEfkLbJkaokw1RgBw3CnVz6Uc.', 'InActivo', 'User', 'Example', '9012345', '79012345', 'Ciudadano', DATEADD(DAY, -60, GETDATE()), 1, DATEADD(DAY, -90, GETDATE()), 1, DATEADD(DAY, -30, GETDATE()), 'InActivo', 0);

-- Volume testing citizens
INSERT INTO [User] (Email, Password, FirstName, FirstLastName, SecondLastName, NationalID, Phone, Role, LastAccess, CreatedBy, CreationDate, Status, MustChangePassword)
VALUES 
('patricia.alvarez@gmail.com', '$2a$11$Qs5oav2FaHcgI6rXehpb8Oa1iFKgEfkLbJkaokw1RgBw3CnVz6Uc.', 'Patricia', 'Alvarez', 'Mamani', '1122334', '71122334', 'Ciudadano', DATEADD(DAY, -3, GETDATE()), 1, DATEADD(DAY, -60, GETDATE()), 'Activo', 0),
('miguel.herrera@hotmail.com', '$2a$11$Qs5oav2FaHcgI6rXehpb8Oa1iFKgEfkLbJkaokw1RgBw3CnVz6Uc.', 'Miguel', 'Herrera', 'Villarroel', '2233445', '72233445', 'Ciudadano', DATEADD(HOUR, -12, GETDATE()), 1, DATEADD(DAY, -25, GETDATE()), 'Activo', 0),
('valeria.castro@yahoo.com', '$2a$11$Qs5oav2FaHcgI6rXehpb8Oa1iFKgEfkLbJkaokw1RgBw3CnVz6Uc.', 'Valeria', 'Castro', 'Pinto', '3344556', '73344556', 'Ciudadano', DATEADD(MINUTE, -45, GETDATE()), 1, DATEADD(DAY, -8, GETDATE()), 'Activo', 0),
('diego.mamani@outlook.com', '$2a$11$Qs5oav2FaHcgI6rXehpb8Oa1iFKgEfkLbJkaokw1RgBw3CnVz6Uc.', 'Diego', 'Mamani', 'Choque', '4455667', '74455667', 'Ciudadano', DATEADD(DAY, -7, GETDATE()), 1, DATEADD(DAY, -40, GETDATE()), 'Activo', 0),
('fernanda.quispe@gmail.com', '$2a$11$Qs5oav2FaHcgI6rXehpb8Oa1iFKgEfkLbJkaokw1RgBw3CnVz6Uc.', 'Fernanda', 'Quispe', 'Laura', '5566778', '75566778', 'Ciudadano', DATEADD(HOUR, -8, GETDATE()), 1, DATEADD(DAY, -12, GETDATE()), 'Activo', 0);

GO
