# Users microservice

## Tabla de contenidos

- [Introducción](#introducción)
- [Convenciones](#convenciones)
- [Dependencias](#dependencias)
- [Estructura de carpetas](#estructura-de-carpetas)
- [Configuraciones](#configuraciones)
  - [Identity](#identity)
  - [DataProtectionTokenProvider](#dataprotectiontokenprovider)
  - [JWT](#jwt)
  - [Microservicios externos](#microservicios-externos)
    - [Email](#email)
  - [RouteOptions](#routeoptions)
- [Servicios](#servicios)
  - [Estructura de las carpetas de servicios](#estructura-de-las-carpetas-de-servicios)

## Introducción

Este proyecto tiene como objetivo gestionar los productos de Visual Contact, ofreciendo funcionalidades para visualizar las distintas categorías de productos disponibles y acceder a una vista detallada de los precios manejados por la empresa. Además, permite crear presupuestos y calcular el precio total de los productos seleccionados.

El proyecto también incluye un módulo de gestión de usuarios y autenticación, que permite a los usuarios iniciar sesión, cerrar sesión y cambiar sus contraseñas.

Se utilizo para la base de datos SQL Server con EntityFramework Core con IdentityUser y IdentityRole para la gestion de usuarios, utilizando un enfoque de base de datos de codigo primero ([Code First](https://learn.microsoft.com/es-es/ef/ef6/modeling/code-first/workflows/new-database)).

Para la autenticacion se utilizo JWT (JSON Web Token) con Authentication.JwtBearer.

## Convenciones

El codigo de este microservicio sigue las siguientes convenciones:

- Todo archivo de codigo fuente y/o configuracion relacionado con este microservicio esta escrito en ingles, para mantener la consistencia del codigo y seguir las buenas practicas de desarrollo de software.

- Documentación del microservicio se realiza en español, debido a que el microservicio esta destinado a ser utilizado por el equipo de desarrollo de Visual Contact.
  
- Se aplican los siguientes patrones de diseño:
  - Patrón Repository: Se utiliza para separar la lógica de acceso a datos de la lógica de negocio. Los repositorios encapsulan la lógica de acceso a la base de     datos y proporcionan métodos para realizar operaciones CRUD en las entidades de la base de datos de manera uniforme.
    
  - Patrón Unit of Work: Se utiliza para coordinar múltiples operaciones de repositorio en una única transacción. El Unit of Work es responsable de agrupar          operaciones de repositorio relacionadas en una transacción única, garantizando la coherencia y la atomicidad de las operaciones de base de datos.
  - Result Pattern: una forma de representar el resultado de una operación, ya sea exitosa o haya encontrado un error, de una manera más explícita y estructurada

- Se utiliza GitFlow como metodologia de trabajo, para mas informacion sobre GitFlow puede visitar el siguiente enlace [GitFlow](https://www.atlassian.com/es/git/tutorials/comparing-workflows/gitflow-workflow).

- [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions)

## Dependencias

- [.NET 8 LTS](https://dotnet.microsoft.com/download/dotnet/8.0)
- [EntityFramework Core](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/)
- [IdentityUser Core](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity.EntityFrameworkCore/)
- [IdentityRole Core](https://www.nuget.org/packages/Microsoft.AspNetCore.Identity.EntityFrameworkCore/)
- [Authentication.JwtBearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer/)
- [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore/)

## Estructura de carpetas

```
└── 📁PricingCalculator
    └── 📁Clients
        └── ...
    └── 📁Configurations
        └── ...
    └── 📁Controllers
        └── ...
    └── 📁Data
        └── 📁Repositories
            └── 📁...
                └── Interface.cs
                └── Repository.cs
            └── GenericRepository.cs
            └── IGenericRepository.cs
        └── DataContext.cs
        └── IUnitOfWork.cs
        └── UnitOfWork.cs
    └── Dockerfile
    └── 📁Dtos
        └── BaseDto.cs
        └── 📁Requests
            └── ...
        └── 📁Responses
            └── ...
    └── 📁Helpers
    └── 📁Migrations
        └── ...
    └── 📁Models
        └── ...
    └── Program.cs
    └── 📁Properties
        └── ...
    └── 📁Services
        └── 📁...
            └── Interface.cs
            └── Service.cs
```

- **Clients**: En esta carpeta se encuentran las clases responsables de interactuar con los microservicios externos a través de HttpClient. Estas clases encapsulan la lógica de comunicación con los servicios externos y manejan las solicitudes y respuestas HTTP correspondientes.

- **Configurations**: En esta sección se encuentran las clases encargadas de configurar servicios, utilidades y dependencias necesarias para el correcto funcionamiento del microservicio. Aquí se definen opciones de configuración, inyecciones de dependencias y otras configuraciones relevantes para la aplicación.

- **Controllers**: Contiene los controladores de la API (End points), que se encargan de recibir las peticiones HTTP y devolver las respuestas HTTP.

- **Data**: Contiene la clase que se encarga de la conexion con la base de datos y la gestión de los repositorios de datos.
  
- **Repositories**: Contiene los repositorios que proporcionan una capa de abstracción sobre la capa de datos subyacente. Estos repositorios encapsulan la lógica de acceso a datos y proporcionan métodos para realizar operaciones CRUD (Crear, Leer, Actualizar, Eliminar) en las entidades de la base de datos.

- **UnitOfWork**: Este archivo se encarga de coordinar las operaciones de persistencia de datos en la base de datos. El Unit of Work es responsable de agrupar múltiples operaciones de repositorio en una única transacción, garantizando la coherencia y la atomicidad de las operaciones de base de datos.

- **Dtos**: Contiene las clases que representan los objetos de transferencia de datos (DTO) utilizados para modelar los datos enviados y recibidos en las solicitudes HTTP.

- **Helpers**: Contiene clases con funciones y utilidades que son compartidas entre diferentes partes del código base. Estas funciones proporcionan funcionalidades comunes que son utilizadas en múltiples lugares del proyecto, evitando así la repetición de código y promoviendo la reutilización.

- **Migrations**: Contiene las migraciones de base de datos, que son scripts que describen los cambios en la estructura de la base de datos a lo largo del tiempo.

- **Models**: las clases que representan las entidades de la base de datos y otros modelos de datos utilizados en la aplicación. Estas clases definen la estructura y el comportamiento de los datos almacenados en la base de datos, así como otras clases de utilidad necesarias para la aplicación.

- **Services**: Contiene las clases que se encargan de la logica de negocio de la aplicacion.

## Configuraciones

### Identity

Este microservicio utiliza Identity para la gestion de usuarios, por lo que es necesario configurar Identity en el archivo `Program.cs` de la siguiente manera:

```csharp
    builder.Services.AddIdentity<User, VisualCrmRole>(options =>
        {
            // Cuztomization of the password requirements and user requirements.
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<DataContext>()
        .AddDefaultTokenProviders();
```

Identity utiliza dos clases para la gestion de usuarios, `User` y `Role`, estas clases se encuentran en la carpeta `Models` y heredan de `IdentityUser` y `IdentityRole` respectivamente.

Para profundizar mas sobre Identity puede visitar el siguiente enlace [ASP.NET Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-8.0&tabs=visual-studio).

### DataProtectionTokenProvider

Para la generacion de tokens de confirmacion de correo electronico y restablecimiento de contraseña se utiliza `DataProtectionTokenProvider`, para configurar este servicio se debe agregar el siguiente codigo en el archivo `Program.cs`:

```csharp
    builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    {
        options.TokenLifespan = TimeSpan.FromHours(2);
    });
```

Donde `TokenLifespan` es el tiempo de vida del token, en este caso es de 2 horas.

### JWT

Para la autenticacion se utiliza JWT (JSON Web Token) con Authentication.JwtBearer, para configurar este servicio se debe agregar el siguiente codigo en el archivo `Program.cs`:

```csharp
    builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateActor = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        RequireExpirationTime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("Jwt:Key").Value!)
            ),
        ClockSkew = TimeSpan.Zero
    };
});
```

Seguido de esto se debe añadir el middleware de autenticacion en el archivo `Program.cs`:

```csharp
    app.UseAuthentication();
    app.UseAuthorization();
```

> Importante: El middleware de autenticacion debe ir antes del middleware de autorizacion.

### Microservicios externos

#### Email

Para el envio de correos electronicos se utilizo el microservicio de Email, para ello se creo una clase `EmailClient.cs` que se encarga de consumir el microservicio de Email, esta clase contiene los siguientes metodos:

```csharp
        public async Task SendEmailAsync(EmailMessage message)
        {
            // ...
        }
```

Este cliente debe ser configurado en el archivo `Program.cs` usando `AddHttpClient` que nos provee .NET Core para la inyeccion de dependencias.

```csharp
    builder.Services.AddHttpClient<IEmailClient,EmailClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration.GetSection("Microservices:Email:BaseUrl").Value!);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    });
```

Donde `Microservices:Email:BaseUrl` es la url base del microservicio de Email, que esta configurada en el archivo `appsettings.json`.

```json
{
  "Microservices": {
    "Email": {
      "BaseUrl": "EMAIL_MICROSERVICE_BASE_URL"
    }
  }
}
```

### RouteOptions

Para la configuracion de las rutas de los controladores se utiliza `RouteOptions`, para configurar este servicio se debe agregar el siguiente codigo en el archivo `Program.cs`:

```csharp
    builder.Services.Configure<RouteOptions>(options =>
    {
        options.LowercaseUrls = true;
        options.LowercaseQueryStrings = false;
    });
```

Esto permite que las rutas de los controladores sean en minusculas y que los query strings no sean en minusculas.

Por ejemplo, si se tiene el siguiente controlador:

```csharp
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // ...
    }
```

La ruta de este controlador seria `api/users`, en minusculas, y si la ruta tuviera un query string, este no seria en minusculas, por ejemplo `api/users?userId=1&token=ff133asd`.


para profundizar mas sobre RouteOptions puede visitar el siguiente enlace [RouteOptions](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.routing.routeoptions?view=aspnetcore-8.0).

## Servicios

### Introducción
En esta sección se detalla los diferentes servicios que hay disponibles en el proyecto.

### Estructura de las carpetas de servicios

```
└── 📁PricingCalculator
    └── 📁Services
        └── 📁AuthServices
            └── IAuthenticationService.cs
            └── AuthenticationService.cs
        └── 📁TokenServices
            └── ITokenService.cs
            └── TokenService.cs
        └── 📁ProductServices
            └── IProductService.cs
            └── ProductService.cs
        └── 📁CategoryServices
            └── ICategoryService.cs
            └── CategoryService.cs    
        └── 📁UserServices
            └── IUserService.cs
            └── UserService.cs
```
Dentro de la carpeta Services están todos los servicios que hacen parte de la solución, separados por carpetas de acuerdo a las entidades que se ven afectadas.

### AuthServices: 
Contiene los metodos que se encargan de gestionar la autenticación de la aplicación.  

- Login
  ```csharp
    Task<Result<LoginResponse, Error>> Login(LoginRequest request);
  ```
  - Descripción: Este método permite a un usuario iniciar sesión en el sistema utilizando las credenciales proporcionadas en la solicitud de inicio de sesión.
  - Parámetros:
    - `request`: La solicitud de inicio de sesión que contiene las credenciales del usuario.  
  - Devoluciones: Devuelve un objeto `Result<LoginResponse, Error>` que contiene un objeto `LoginResponse` con la información de inicio de sesión, como el token de acceso, refresh token e información del usuario logueado si la operación se realiza con éxito, o un error si ocurre algún problema.

- Logout
  ```csharp
    Task<Result<string, Error>> Logout();
  ```
  - Descripción: Este método permite a un usuario cerrar sesión en el sistema, invalidando el refresh token actual, impidiendo obtener un nuevo token de acceso.
  - Parámetros:
    - `request`: La solicitud de inicio de sesión que contiene las credenciales del usuario.  
  - Devoluciones: Devuelve un objeto `Result<string, Error>` que contiene un mensaje de confirmación si la operación se realiza con éxito, o un error si ocurre algún problema.

- CreateRefreshToken
  ```csharp
    Task<Result<AuthenticatedUserResponse, Error>> CreateRefreshToken(RefreshRequest refreshRequest)
  ```
- Descripción: Este método permite generar un nuevo token de acceso y refresh token para un usuario autenticado utilizando un token de actualización proporcionado.
- Parámetros:
  - `refreshRequest`: La solicitud de actualización que contiene el token de actualización del usuario.
- Devoluciones: Devuelve un objeto Result<AuthenticatedUserResponse, Error> que contiene un objeto AuthenticatedUserResponse con la información de autenticación actualizada, como el nuevo token de acceso, refresh token e información del usuario si la operación se realiza con éxito, o un error si ocurre algún problema.

### TokenServices:
Contiene los metodos que se encargan de la generación y validación de tokens de autenticación y tokens de actualización

- TokenProcesor
  ```csharp
    Task<AuthenticatedUserResponse> ProcessTokenAsync(User user);
  ```
  - Descripción: Este método maneja la generación del token de acceso y el refresh token y almacena el refresh token en memoria.
  - Parámetros:
    - `user`: El usuario autenticado para el que se procesará los token. 
  - Devoluciones: Devuelve un objeto `AuthenticatedUserResponse` que contiene el token de acceso y el refresh token.

- GenerateToken
  ```csharp
    Task<string> GenerateToken(User user);
  ```
  - Descripción: Este método genera un token de acceso para un usuario específico.
  - Parámetros:
    - `user`: El usuario para el que se generará el token.
  - Devoluciones: Devuelve el token de acceso generado como una cadena.

- GenerateRefreshToken
  ```csharp
    string GenerateRefreshToken();
  ```
  - Descripción: Este método genera un token de actualización que puede ser utilizado para obtener un nuevo token de acceso una vez que el token actual ha expirado.
  - Parámetros: No recibe parámetros.
  - Devoluciones: Devuelve el token de actualización generado como una cadena.

- GenerateRefreshToken
  ```csharp
    bool Validate(string refreshToken);
  ```
  - Descripción: Este método valida si el token que es pasado por parametro es válido y no ha expirado.
  - Parámetros:
    - `refreshToken`: Token usado para obtener un nuevo token de acceso   
  - Devoluciones: Devuelve un booleano que indica si el token es válido o no.

### UserServices: 
Contiene los metodos que se encargan principalmente de gestionar los usuarios.

- GetAllUsers
  ```csharp
    Task<IEnumerable<User>> GetAllUsers();
  ```
  - Descripción: Este método recupera todos los usuarios almacenados en el sistema.   
  - Parámetros: No recibe parametros.     
  - Devoluciones: Retorna una lista de usuarios `User`.

- GetUserById
  ```csharp
    Task<User?> GetUserById(string id);
  ```
  - Descripción: Recupera un usuario específico por su identificador único.  
  - Parámetros:
    - `id`: Identificador único del usuario que se desea recuperar.     
  - Devoluciones: Retorna el usuario encontrado o null si no existe.

- CreateUser
  ```csharp
    Task<IdentityResult> CreateUser(CreateUserRequest user);
  ```
  - Descripción: Este método crea un nuevo usuario con los datos proporcionados en la solicitud de creación.
  - Parámetros:
    - `user`: Objeto que contiene los datos del nuevo usuario a crear.   
  - Devoluciones: Retorna un objeto IdentityResult que indica el resultado de la operación de actualización.
 
- DeleteUser
  ```csharp
    Task<IdentityResult> DeleteUser(User user);
  ```
  - Descripción: Este método elimina un usuario existente de la base de datos.
  - Parámetros:
    - `user`: Usuario que se desea eliminar.   
  - Devoluciones: Retorna un objeto IdentityResult que indica el resultado de la operación de eliminación.

- FindUserByEmail
  ```csharp
      Task<User?> FindUserByEmail(string email);
  ```
  - Descripción:  Encuentra un usuario por dirección de correo electrónico.
  - Parámetros:
    - `email`: La dirección de correo electrónico del usuario a encontrar.
  - Devoluciones: Devuelve el usuario encontrado o null si no se encuentra ningún usuario con el correo electrónico especificado.

- GetRoles
  ```csharp
    Task<IEnumerable<string>> GetRoles(User user);
  ```
  - Descripción: Obtiene los roles asignados a un usuario.
  - Parámetros:
    - `user`: El usuario del cual se obtendrán los roles.
  - Devoluciones: Devuelve una colección de nombres de roles asociados con el usuario.

- ValidatePassword
  ```csharp
    Task<bool> ValidatePassword(User user, string password);
  ```
  - Descripción: Este método verifica si la contraseña proporcionada coincide con la contraseña almacenada para un usuario específico.
  - Parámetros:
    - `user`: El usuario para el cual se validará la contraseña.
    - `password`: La contraseña a validar.
  - Devoluciones: Retorna `true` si la contraseña es válida para el usuario especificado, de lo contrario, devuelve `false`.

- ChangePassword
  ```csharp
    Task<IdentityResult> ChangePassword(string id, ChangePasswordRequest changePasswordRequest);
  ```
  - Descripción: Este método verifica si la contraseña proporcionada coincide con la contraseña almacenada para un usuario específico.
  - Parámetros:
    - `id`: El ID del usuario para el cual se cambiará la contraseña.
    - `changePasswordRequest`: La solicitud para cambiar la contraseña, que contiene la contraseña antigua y la nueva.
  - Devoluciones: Devuelve un objeto `IdentityResult` que indica si la operación de cambio de contraseña fue exitosa.

- ForgotPassword
  ```csharp
    Task<IdentityResult> ForgotPassword(string email);
  ```
  - Descripción: Envía una url con el token para restablecer la contraseña de un usuario a su correo electrónico.
  - Parámetros:
    - `email`: La dirección de correo electrónico del usuario para el cual se enviará el correo electrónico de restablecimiento de contraseña.
  - Devoluciones: Devuelve un objeto `IdentityResult` que indica si el correo electrónico de restablecimiento de contraseña se envió correctamente.

- ResetPassword
  ```csharp
    Task<IdentityResult> ResetPassword(string userId, string token, ResetPasswordRequest resetPasswordRequest);
  ```
  - Descripción: Este método permite restablecer la contraseña de un usuario utilizando un token de restablecimiento y una nueva contraseña proporcionada.
  - Parámetros:
    - `userId`: El ID del usuario para el cual se restablecerá la contraseña.
    - `token`: El token de restablecimiento de contraseña enviado al usuario.
    - `resetPasswordRequest`: El objeto que contiene la nueva contraseña del usuario.
  - Devoluciones: Devuelve un objeto `IdentityResult` que indica si la operación de restablecimiento de contraseña fue exitosa.

### ProductServices: 
Contiene los metodos que se encargan de gestionar los productos de visual contact.

- CreateProduct
  ```csharp
    Task<Result<Product, Error>> CreateProduct(CreateProductDTO createProductDTO);
  ```
  - Descripción: Crea un nuevo producto utilizando la información proporcionada en el DTO de creación de productos.
  - Parámetros:
    - `createProductDTO`: DTO que contiene la información necesaria para crear un nuevo producto.
  - Devoluciones: Retorna un resultado que contiene el producto creado o un error si la creación falla.

- GetAllProducts
  ```csharp
    Task<Result<List<Product>, Error>> GetAllProducts();
  ```
  - Descripción: Recupera una lista de todos los productos disponibles.
  - Parámetros: No recibe parametros. 
  - Devoluciones: Retorna un resultado que contiene una lista de productos o un error si la recuperación falla.

- CreateProduct
  ```csharp
    Task<Result<Product, Error>> GetProductById(Guid id);
  ```
  - Descripción: Recupera un producto específico por su identificador único.
  - Parámetros:
    - `id`: Identificador único del producto que se desea recuperar.
  - Devoluciones: Retorna un resultado que contiene el producto encontrado o un error si no existe.

### CategoryServices: 
Contiene los metodos que se encargan de gestionar las categorias de los productos que maneja visual contact.

- CreateCategory
  ```csharp
    Task<Result<Category, Error>> CreateCategory(CreateCategoryDTO createCategoryDTO);
  ```
  - Descripción: Crea una nueva categoría utilizando la información proporcionada en el DTO de creación de categorías.
  - Parámetros:
    - `createCategoryDTO`: DTO que contiene la información necesaria para crear una nueva categoría.
  - Devoluciones: Retorna un resultado que contiene la categoría creada o un error si la creación falla.

- GetAllCategoriesWithProducts
  ```csharp
    Task<Result<List<Category>, Error>> GetAllCategoriesWithProducts();
  ```
  - Descripción: Recupera una lista de todas las categorías junto con los productos asociados a cada categoría.
  - Parámetros: No recibe parametros.
  - Devoluciones: Retorna un resultado que contiene una lista de categorías con sus productos o un error si la recuperación falla.

- GetCategoryById
  ```csharp
    Task<Result<Category, Error>> GetCategoryById(int id);
  ```
  - Descripción: Recupera una categoría específica por su identificador único.
  - Parámetros:
    - `id`: Identificador único de la categoría que se desea recuperar.
  - Devoluciones: Retorna un resultado que contiene la categoría encontrada o un error si no existe.