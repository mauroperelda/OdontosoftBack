-- Revierte la migración 20260620120000_AddRolesPermisosUsuarios
-- Borra las 5 tablas RBAC creadas en OdontoSoft.API.
-- Orden inverso a la creación (primero las join tables, después las entidades).

IF OBJECT_ID('FK_UsuarioRol_Usuario_UsuarioId', 'F') IS NOT NULL
    ALTER TABLE UsuarioRol DROP CONSTRAINT FK_UsuarioRol_Usuario_UsuarioId;
IF OBJECT_ID('FK_UsuarioRol_Rol_RolId', 'F') IS NOT NULL
    ALTER TABLE UsuarioRol DROP CONSTRAINT FK_UsuarioRol_Rol_RolId;
IF OBJECT_ID('FK_RolPermiso_Permiso_PermisoId', 'F') IS NOT NULL
    ALTER TABLE RolPermiso DROP CONSTRAINT FK_RolPermiso_Permiso_PermisoId;
IF OBJECT_ID('FK_RolPermiso_Rol_RolId', 'F') IS NOT NULL
    ALTER TABLE RolPermiso DROP CONSTRAINT FK_RolPermiso_Rol_RolId;

IF OBJECT_ID('UsuarioRol', 'U') IS NOT NULL DROP TABLE UsuarioRol;
IF OBJECT_ID('RolPermiso', 'U') IS NOT NULL DROP TABLE RolPermiso;
IF OBJECT_ID('Usuario', 'U')   IS NOT NULL DROP TABLE Usuario;
IF OBJECT_ID('Rol', 'U')       IS NOT NULL DROP TABLE Rol;
IF OBJECT_ID('Permiso', 'U')   IS NOT NULL DROP TABLE Permiso;

PRINT 'Rollback 20260620120000 aplicado correctamente.';
