# OnlineShop
Проект интернет магазина компьютерной техники на ASP.NET Core. <br/>
Использует микросервис отзывов [ReviewsWebApplication](https://github.com/IvanPovaliaev/ReviewsWebApplication) (*данный микросервис добавляет доп. функционал к приложению, его использование не носит обязательный характер*)\
Проект реализован в виде MVC приложения и так же имеет своё WebApi. Проект обёрнут в докер (см. раздел ***Docker***)

![image](https://github.com/user-attachments/assets/cce15af0-08bc-41d8-8235-ac91b94eaae0)
![image](https://github.com/user-attachments/assets/66c49a56-9b91-4f9a-a1b5-9e8ad4d6d366)

# Используемые библиотеки и технологии
1. ASP.NET CORE MVC/WebApi
2. MsSQL/PostgreSQL/MySQL
3. Entity Framework Core
4. Linq
5. ASP.NET Core Identity
6. JWT bearer-based authentication
7. Automapper
8. Postman/Swagger
9. Dependency Injection
10. Scrutor
11. MediatR - для реализации событийной модели
12. Redis
13. Unit-тесты
14. Хеширование данных с использованием алгоритма хеширования Argon2 ([Konscious.Security.Cryptography.Argon2](https://github.com/kmaragon/Konscious.Security.Cryptography))
15. Serilog - логирование данных в файл, консоль и БД
16. MimeKit (работа с электронной почтой)
17. Clean Architecture
18. Docker (в разработке)
19. FeatureFlags

## Docker
Docker-образы проекта:
1. WebApi: https://hub.docker.com/repository/docker/ivanpovaliaev/onlineshop_api/general <br/>
(путь до Swagger по умолчанию `{Scheme}://{ServiceHost}:{ServicePort}/swagger`)
2. MVC: https://hub.docker.com/repository/docker/ivanpovaliaev/onlineshop_mvc/general

Для сборки всего приложения, используйте приведённый compose файл:
```
networks:
    onlineshop:

services:
  postgre:
    container_name: online_shop_app_db
    image: postgres:latest
    environment: 
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: 1Secure*Password1
      POSTGRES_DB: online_shop_db
    ports:
      - 5432:5432
    networks:
      - onlineshop
    volumes:
      - postgres_data:/var/lib/postgresql/data

  redis:
    container_name: redis
    image: redis
    ports:
      - 6379:6379
    restart: unless-stopped 
    networks:
      onlineshop:

  onlineshop.mvc:
    container_name: online_shop_mvc
    image: ivanpovaliaev/onlineshop_mvc:latest
    environment:
      - ASPNETCORE_HTTP_PORTS=8080
    ports:
      - 8080:8080
    depends_on:
      - postgre
      - redis
    restart: unless-stopped
    networks:
      - onlineshop
    volumes:
      - ./OnlineShopWebApp/appsettings.json:/app/appsettings.json:rw
      - ./OnlineShopWebApp/wwwroot/img:/app/wwwroot/img:rw

  onlineshop.webapi:
    container_name: onlineshop_api
    image: ivanpovaliaev/onlineshop_api:latest
    environment:
      - ASPNETCORE_HTTP_PORTS=8090
    build:
      context: .
      dockerfile: OnlineShop.WebAPI/Dockerfile
    ports:
      - "8090:8090"
    depends_on:
      - postgre  
      - redis
    restart: unless-stopped
    networks:
      - onlineshop

  mssqlserver:
    container_name: reviews_db
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment: 
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=1Secure*Password1
    volumes:
      - mssql_data:/var/opt/mssql
    ports:
      - "1433:1433"
    networks:
      - onlineshop
    
  reviews.api:
    container_name: reviews_api
    image: ivanpovaliaev/reviews_api:latest
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:7274;
    ports:
      - "7274:7274"
    depends_on:
      - mssqlserver   
    restart: unless-stopped
    networks:
      - onlineshop

volumes:
    mssql_data:
    postgres_data:

```

## Кеширование
Для получения данных каталога используется Redis, что улучшает производительность и уменьшает нагрузку на базу данных. <br/>
Redis интегрирован через декоратор, что позволяет не изменять существующую функциональность.\
*Установка Redis носит опциональный характер. Приложение работает и без него*

## Тестирование
Проект (OnlineShopWebApp и Application) покрыт тестами с использованием xUnit, Bogus и Moq.\
Для удобства тестирования, в проект тестов был интегрирован Dependency Injection от Microsoft

# Основные функции
**Товары:** пользователям магазина доступен просмотр каталога товаров по категориям, подробной информации о товаре, добавление товаров в избранное, сравнении и корзину.<br/>
**Пользователи:** авторизованные пользователи могут сбросить свой пароль, изменить личную информацию, просмотреть историю своих заказов.<br/>
**Администратор:** администратору доступны функции по работе с товарами, ролями, пользователями, изменением статуса заказов и по функциональности MVC приложения (см. раздел **FeatureFlags**)<br/>
**Заказы**: пользователи могу оформить свой заказ и просмотреть его статус в личном кабинете.<br/>
**Сброс пароля**: все пользователи могут сбросить свой текущий пароль. Инструкция по сбросу пароля придёт на указанную при регистрации почту.<br/>
Приложения **WebApi** и **MVC** используют общую БД и получают согласованные между собой данные. Могут работать независимо друг от друга.<br/>

## FeatureFlags

MVC приложение поддерживает работу FeatureFlags для отключения микросервиса отзывов, заказов в личном кабинете пользователя и новогодней тематики сайта.

---
> [!NOTE]
> Имеется возможность настройки сторонник сервисов для работы с флагами.\
> Пример для ConfigCat имеется в приложении:\
> Расскомментируйте `//builder.Services.AddSingleton<IFeatureDefinitionProvider, ConfigCatFeatureDefinitionProvider>();` в `Program.cs` и добавьте свой SDK-ключ в appsettings.json в секции `ConfigCatOptions`.\
> При использовании сторонних сервисов работы с флагами, секцию администратора `Настройки`, рекомедуется отключить.
---

# Установка и настройка

Для работа приложения достаточно иметь любую установленную СУБД (MsSQL, MySQL, PostgreSQL) и настроить подключение в `appsettings.json` (у WebApi и MVC)
```
  "DatabaseProvider": "online_shop_MsSQL",
  "ConnectionStrings": {
    "online_shop_PostgreSQL": "Host=localhost;Port=5432;Database=online_shop_povaliaev;Username=postgres;Password=Qwerty123;",
    "online_shop_MsSQL": "Server=(localdb)\\mssqllocaldb;Database=online_shop_povaliaev;Trusted_Connection=True;",
    "online_shop_MySQL": "Server=localhost;User ID=root;Password=Qwerty123!;Database=online_shop_povaliaev;"
  },
  "DatabaseTypes": {
    "online_shop_PostgreSQL": "PostgreSQL",
    "online_shop_MsSQL": "MsSQL",
    "online_shop_MySQL": "MySQL"
  }
```

В `ConnectionStrings` указать строку подключения / изменить существующие\
В `DatabaseTypes` указать тип БД.\
В `DatabaseProvider` указать имя строки подключение из ConnectionStrings\
P.S. Для MsSQL достаточно настроек по умолчанию.

Дополнительно можно настроить:
1. Начальные данные администратора  в файле `appsettings.json` в секции `AdminSettings`
2. Настройки почтового клиента в секции `MailSettings`
3. Настройки подключения к микросервису отзывов в секции `Microservices:ReviewsService` в случае изменения адреса или запуска ReviewsService через докер контейнер (адрес отличается)
4. Настройки Redis в `RedisSettings`

# Структура каталога решения
![image](https://github.com/user-attachments/assets/7f2480d2-eb11-464b-be35-7aa0b1d26b75)
![image](https://github.com/user-attachments/assets/9b9231f7-b945-40f7-bc14-8e5da39273c2)
