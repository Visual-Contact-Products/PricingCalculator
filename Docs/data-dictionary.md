# Bases de datos
Para el sistema se utiliza una base de datos llamada VCProducts, dicha base de datos contiene las tablas y procedimientos que permiten ejecutar las acciones propias del aplicativo.

# Modelo entidad relación

En el siguiente enlace se encontrará el modelo entidad relación de la base de datos.

https://lucid.app/lucidchart/0b6250a8-b350-4387-a669-3c7d42e5712c/edit?viewport_loc=-761%2C-233%2C3269%2C1417%2C0_0&invitationId=inv_609cf7ee-4fd8-4617-8518-53e161e2891c

## Tabla: Users

## Descripción

Tabla que almacena los usuarios de la aplicación, esta tabla es utilizada para la autenticación y autorización de los usuarios, se utiliza Identity Framework para la gestión de usuarios.

## Campos

| Nombre               | Tipo      | Descripción                                                          |
| -------------------- | --------- | -------------------------------------------------------------------- |
| Id                   | string    | Identificador único del usuario (GUID).                              |
| UserName             | string    | Nombre de usuario, se obtiene a partir del correo electronico.       |
| NormalizedUserName   | string    | Nombre de usuario normalizado en mayusculas.                         |
| Email                | string    | Correo electronico del usuario.                                      |
| NormalizedEmail      | string    | Correo electronico normalizado en mayusculas.                        |
| EmailConfirmed       | bool      | Indica si el correo electronico del usuario esta confirmado.         |
| PasswordHash         | string    | Hash de la contraseña del usuario.                                   |
| SecurityStamp        | string    | Sello de seguridad del usuario.                                      |
| ConcurrencyStamp     | string    | Sello de concurrencia del usuario.                                   |
| PhoneNumber          | string    | Numero de telefono del usuario.                                      |
| PhoneNumberConfirmed | bool      | Indica si el numero de telefono del usuario esta confirmado.         |
| TwoFactorEnabled     | bool      | Indica si el usuario tiene habilitado la autenticacion en dos pasos. |
| LockoutEnd           | datetime2 | Fecha de desbloqueo del usuario.                                     |
| LockoutEnabled       | bool      | Indica si el usuario esta bloqueado.                                 |
| AccessFailedCount    | int       | Contador de intentos fallidos de acceso.                             |

# Tabla: UserRoles

## Descripción

Tabla que almacena los roles de los usuarios, donde se relaciona el usuario con el rol.

## Campos

| Nombre | Tipo   | Descripción                |
| ------ | ------ | -------------------------- |
| UserId | string | Identificador del usuario. |
| RoleId | string | Identificador del rol.     |

# Tabla: Roles

## Descripción

Tabla que almacena los roles de la aplicación.

## Campos

| Nombre           | Tipo   | Descripción                               |
| ---------------- | ------ | ----------------------------------------- |
| Id               | string | Identificador único del rol (GUID).       |
| Name             | string | Nombre del rol.                           |
| NormalizedName   | string | Nombre del rol normalizado en mayusculas. |
| ConcurrencyStamp | string | Sello de concurrencia del rol.            |
| Description      | string | Descripción del rol.                      |

# Tabla: AspNetUserLogins

## Descripción

Tabla que almacena los logins de los usuarios, esta tabla es para la autenticación mediante proveedores externos como Google, Facebook, etc.

## Campos

| Nombre              | Tipo   | Descripción                 |
| ------------------- | ------ | --------------------------- |
| ProviderDisplayName | string | Nombre del proveedor.       |
| UserId              | string | Identificador del usuario.  |
| LoginProvider       | string | Proveedor de autenticación. |
| ProviderKey         | string | Llave del proveedor.        |

# Tabla: Products

## Descripción

Esta tabla almacena los productos ofrecidos por visual contact.

## Campos

| Nombre            | Tipo    | Descripción                                                     |
| ----------------- | ------- | ----------------------------------------------------------------|
| Id                | Guid    | Identificador único del producto.                               |
| Sku               | string  | Codigo único que identifica el producto.                        |
| Description       | string  | Nombre del producto.                                            |
| Price             | float   | Precio del producto.                                            |
| IsActive          | bool    | Indica si el producto está activo o no.                         |
| CategoryId        | float   | Idenficador de la categoría a la que pertenece el producto.     |

# Tabla: Categories

## Descripción

Esta tabla almacena las categorias de los productos de visual contact.

## Campos

| Nombre            | Tipo    | Descripción                                                     |
| ----------------- | ------- | ----------------------------------------------------------------|
| Id                | Guid    | Identificador único de la categoria.                            |
| Name              | string  | Nombre de la categoría.                                         |