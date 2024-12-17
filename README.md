# OnlineShop
Проект интернет магазина компьютерной техника на ASP.NET Core. <br/>
Использует микросервис отзывов [ReviewsWebApplication](https://github.com/IvanPovaliaev/ReviewsWebApplication) (данный микросервис добавляют доп. функционал к приложению, его использование не носит обязательный характер)
Проект реализован в виде MVC приложения и так же имеет своё WebApi.

# Используемые библиотеки и технологии
1. ASP.NET CORE MVC/WebApi
2. MsSQL/PostgreSQL/MySQL
3. Entity Framework Core
4. Linq
5. ASP.NET Core Identity (MVC)
6. JWT bearer-based authentication (Api)
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
18. Docker

## Docker

***раздел в разработке. Скоро тут появится docker-compose и ссылки на образы на docker.hub***

## Кеширование
Для получения данных каталога используется Redis, что улучшает производительность и уменьшает нагрузку на базу данных.
Redis интегрирован через декоратор, что позволяет не изменять существующую функциональность сервиса в слоё Application

## Тестирование
Проект (OnlineShopWebApp и Application) покрыт тестами с использованием xUnit, Bogus и Moq. Для удобства тестирования, в проект тестов был интегрирован Dependency Injection от Microsoft
