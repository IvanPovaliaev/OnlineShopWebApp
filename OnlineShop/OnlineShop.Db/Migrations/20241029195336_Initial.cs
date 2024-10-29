﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OnlineShop.Db.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Cost = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    SpecificationsJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CanBeDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDeliveryInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Entrance = table.Column<int>(type: "integer", nullable: true),
                    Floor = table.Column<int>(type: "integer", nullable: true),
                    Apartment = table.Column<int>(type: "integer", nullable: true),
                    PostCode = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    ReservePhone = table.Column<string>(type: "text", nullable: true),
                    AdditionalInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDeliveryInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartPosition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    CartId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartPosition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartPosition_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartPosition_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComparisonProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComparisonProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComparisonProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    InfoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_UserDeliveryInfo_InfoId",
                        column: x => x.InfoId,
                        principalTable: "UserDeliveryInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderPosition",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderPosition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderPosition_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderPosition_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Cost", "Description", "ImageUrl", "Name", "SpecificationsJson" },
                values: new object[,]
                {
                    { new Guid("0025491a-1eeb-4e6d-8dc9-26fc69ecb1b0"), 4, 7030m, "Жесткий диск Seagate Barracuda 2TB (ST2000DM008) является идеальным решением для хранения больших объемов данных, обеспечивая вместительность и надежность. С интерфейсом SATA-III и скоростью вращения 7200 об/мин, этот диск предлагает отличную производительность для вашего компьютера или сервера.\r\n\r\nОбъем в 2TB позволяет хранить огромное количество файлов – будь то фотографии, видео, музыка или документы. Благодаря высокой скорости передачи данных, загрузка программ и доступ к файлам происходит быстро и эффективно.\r\n\r\nПродукт отличается высокой надежностью, что подтверждается долгим сроком службы и стабильностью работы. Seagate Barracuda – это выбор многих профессионалов, ценящих качество и долговечность.\r\n\r\nЭтот жесткий диск подходит как для домашних пользователей, так и для профессионалов, которым требуется расширенное дисковое пространство для сложных задач. Он легко устанавливается в большинство настольных компьютеров, делая процесс модернизации простым и удобным.\r\n\r\nВыбирая Seagate Barracuda 2TB, вы получаете высокую производительность, надежность и большой объем хранения по доступной цене. Это идеальный вариант для тех, кто ищет эффективное и долгосрочное решение для хранения данных.", "/img/products/2Tb-SATA-III-Seagate-Barracuda.webp", "2Tb SATA-III Seagate Barracuda (ST2000DM008)", "{\"Manufacturer\":\"Seagate\",\"ManufacturerCode\":\"ST2000DM008\",\"FormFactor\":\"3.5\\\"\",\"Interface\":\"SATA-III\",\"Capacity\":\"2000 Гб\"}" },
                    { new Guid("202d752c-be4e-4f32-a7a0-433bcc1f2bb4"), 5, 14160m, "Оперативная память Kingston FURY Beast Black ориентирована на использование в составе мощных ПК игрового уровня. Она выделяется алюминиевыми радиаторами черного цвета, конструкция которых способствует быстрому и стабильному отведению тепла для защиты от перегрева. В комплекте поставляются два модуля объемом по 16 ГБ, которые соответствуют стандарту DDR5 и работают на частоте 6000 МГц. Микросхема управления питанием повышает стабильность работы модулей Kingston FURY Beast. Для сохранения целостности данных реализована функция определения и исправления ошибок ECC.", "/img/products/32Gb DDR5 6000MHz Kingston Fury Beast (KF560C40BBK2-32) (2x16Gb KIT).webp", "32Gb DDR5 6000MHz Kingston Fury Beast (KF560C40BBK2-32) (2x16Gb KIT)", "{\"Manufacturer\":\"Kingston\",\"ManufacturerCode\":\"KF560C40BBK2-32\",\"FormFactor\":\"DIMM\",\"MemoryType\":\"DDR5\",\"MemorySize\":\"32 Гб\",\"ModulesCount\":\"2\",\"ClockSpeed\":\"6000 МГц\"}" },
                    { new Guid("3b232651-c925-4eff-a481-e9d09b503377"), 6, 8250m, "Блок питания 750W Be Quiet System Power 10 - это надежное и мощное устройство, которое отлично подойдет для сборки компьютера любого уровня сложности. Блок питания имеет стандарт ATX12V 2.52 и активный PFC, что обеспечивает стабильное энергоснабжение и защиту от перенапряжений.\r\n\r\nОдной из особенностей этого блока питания является наличие встроенного вентилятора диаметром 120 мм, который обеспечивает эффективное охлаждение и позволяет поддерживать низкие температуры внутри корпуса компьютера. Это особенно важно при работе с высоконагруженными приложениями и играми.\r\n\r\nБлок питания 750W Be Quiet System Power 10 оснащен различными разъемами для подключения различных компонентов компьютера. На материнской плате предусмотрен разъем 20+4 pin, а также по одному разъему 4-pin и 4+4-pin CPU. Для видеокарт имеется 4 разъема 6+2-pin PCI-E, что позволяет подключить несколько мощных графических ускорителей.\r\n\r\nТакже блок питания обладает разъемами 4-pin IDE (Molex) и 15-pin SATA для подключения различных устройств хранения данных, оптических приводов и других компонентов. Это удобно и позволяет создать функциональную систему хранения данных.\r\n\r\nБлок питания 750W Be Quiet System Power 10 имеет сертификат 80 PLUS Bronze, что гарантирует высокую энергоэффективность устройства и экономичное потребление электроэнергии. Это позволяет снизить затраты на электроэнергию и уменьшить нагрузку на систему охлаждения компьютера.\r\n\r\nДлина кабеля питания материнской платы составляет 55 см, что обеспечивает удобство монтажа и позволяет грамотно организовать внутреннюю разводку кабелей в корпусе компьютера.\r\n\r\nБлок питания 750W Be Quiet System Power 10 - это надежное и производительное устройство, которое гарантирует стабильную работу компьютера даже при высоких нагрузках. Благодаря своим характеристикам и функционалу, этот блок питания станет отличным выбором для тех, кто ценит качество и эффективность.", null, "750W Be Quiet System Power 10", "{\"Manufacturer\":\"Be Quiet\",\"ManufacturerCode\":\"BN329\",\"Power\":\"750 Вт\",\"PFC\":\"активный\",\"FanSize\":\"120 мм\"}" },
                    { new Guid("5c2a6e43-0e05-47a2-9c92-eb839f0b3e63"), 5, 11870m, "Представляем вашему вниманию уникальный продукт - оперативная память Team T-Create Expert DDR5 32Gb 6000MHz. Этот модуль отличается не только высокой производительностью, но и элегантным дизайном, который подойдет к любому современному компьютеру.\r\n\r\nОперативная память имеет форм-фактор DIMM и работает на частоте 6000 МГц, что обеспечивает высокую скорость передачи данных. С тактовой частотой 6000 МГц и пропускной способностью 48000 Мб/с, эта память позволит вам запускать самые требовательные приложения и игры без задержек и лагов.\r\n\r\nВ комплекте поставляется 2 модуля по 16 Гб каждый, что общим объемом составляет 32 Гб. Это идеальное решение для тех, кто ценит производительность и мощность своего компьютера.\r\n\r\nКроме того, данная оперативная память имеет низкую задержку CAS Latency (CL) 38, что также положительно сказывается на скорости работы системы. RAS to CAS Delay (tRCD) составляет 38, Row Precharge Delay (tRP) - 38, а Activate to Precharge Delay (tRAS) - 78, что гарантирует стабильную и эффективную работу памяти.\r\n\r\nОхлаждение модулей осуществляется пассивным радиатором, который обеспечивает надежную защиту от перегрева и позволяет поддерживать низкую температуру даже при интенсивной работе. Это позволяет увеличить срок службы оперативной памяти и обеспечить ее стабильную работу в любых условиях.\r\n\r\nTeam T-Create Expert DDR5 32Gb 6000MHz - это идеальный выбор для тех, кто ценит качество, производительность и надежность. Позвольте вашему компьютеру работать на полную мощность с этим уникальным модулем оперативной памяти!", "/img/products/32Gb-DDR5-6000MHz-Team-T-Create-Expert-_2x16Gb-KIT.webp", "32Gb DDR5 6000MHz Team T-Create Expert (CTCED532G6000HC38ADC01) (2x16Gb KIT)", "{\"Manufacturer\":\"Team\",\"ManufacturerCode\":\"CTCED532G6000HC38ADC01\",\"FormFactor\":\"DIMM\",\"MemoryType\":\"DDR5\",\"MemorySize\":\"32 Гб\",\"ModulesCount\":\"2\",\"ClockSpeed\":\"6000 МГц\"}" },
                    { new Guid("c643c046-93df-4d0f-ba01-224b689eca0f"), 1, 15870m, "Процессор Intel Core i5-12400F OEM\r\n\r\nИщете эффективное и современное решение для вашего компьютера? Процессор Intel Core i5-12400F — ваш идеальный выбор! Эта модель прекрасно подходит как для повседневных задач, так и для игр и программ, требующих высокой производительности.\r\n\r\nОсновные характеристики:\r\n- Количество ядер: 6\r\n- Количество потоков: 12\r\n- Базовая частота: 2.5 ГГц\r\n- Максимальная турбо-частота: 4.4 ГГц\r\n- Тип памяти: поддержка DDR4 и DDR5\r\n- Сокет: LGA 1700\r\n\r\nПреимущества:\r\n- Высокая производительность: Благодаря шести ядрам и двенадцати потокам, этот процессор обеспечивает отличную многоядерную производительность, что делает его отличным выбором для многозадачности и требовательных приложений.\r\n- Энергоэффективность: Intel Core i5-12400F оптимизирован для эффективного потребления энергии, что помогает снизить нагрев и увеличить срок службы компонентов.\r\n- Поддержка современных технологий: Процессор поддерживает последние стандарты памяти DDR4 и DDR5, что обеспечивает улучшенную производительность и скорость передачи данных.\r\n- Совместимость: Сокет LGA 1700 гарантирует широкую совместимость с новейшими материнскими платами.\r\n\r\nВыбирая Intel Core i5-12400F, вы получаете надежный и мощный процессор, который прослужит вам долгие годы. Улучшите свой компьютер сегодня и наслаждайтесь быстродействием и стабильностью на новом уровне!", "/img/products/Intel-Core-i5-12400F-OEM.webp", "Intel Core i5 - 12400F OEM", "{\"Manufacturer\":\"Intel\",\"ManufacturerCode\":\"CM8071504555318/CM8071504650609\",\"Model\":\"Core i5 12400F\",\"Socket\":\"LGA 1700\",\"Architecture\":\"Alder Lake\",\"CoresCount\":\"6\",\"ThreadsCount\":\"12\",\"ClockSpeed\":\"2500 МГц\"}" },
                    { new Guid("d13b5481-bfe0-4d5c-885b-1d43df8aa6b9"), 5, 20790m, "Оперативная память 64Gb DDR5 5600MHz ADATA XPG Lancer (AX5U5600C3632G-DCLABK) (2x32Gb KIT) – это высокопроизводительный комплект памяти, который обеспечит быструю и надежную работу вашего компьютера. Этот набор памяти состоит из двух модулей по 32 Гб каждый, общий объем которых составляет 64 Гб.\r\n\r\nОсновные характеристики этой оперативной памяти включают в себя форм-фактор DIMM, тип памяти DDR5 и тактовую частоту 5600 МГц. С такими параметрами, эта память обладает высокой пропускной способностью в 44800 Мб/с, что позволяет быстро обрабатывать данные и выполнять сложные вычисления.\r\n\r\nПомимо этого, оперативная память ADATA XPG Lancer имеет низкую задержку CAS Latency (CL) в 36 тактов, что также сказывается на скорости работы системы. Система охлаждения этой памяти выполнена в виде пассивного радиатора, который обеспечивает эффективное отвод тепла и защиту от перегрева.\r\n\r\nОсобенностью этой оперативной памяти является также наличие активной системы охлаждения с вентилятором. Радиатор изготовлен из меди, что обеспечивает отличное теплопроводность. Вентилятор имеет размер 80x80 мм и оснащен подшипником скольжения для бесшумной работы. Регулятор оборотов PWM позволяет настраивать скорость вращения вентилятора в зависимости от температуры памяти.\r\n\r\nЭтот комплект оперативной памяти ADATA XPG Lancer предназначен для использования в системах, где требуется высокая производительность и стабильная работа. Благодаря высокой тактовой частоте и эффективной системе охлаждения, эта память подойдет как для геймеров, так и для профессиональных пользователей, занимающихся ресурсоемкими задачами.\r\n\r\nЕсли вам необходима надежная и мощная оперативная память для вашего компьютера, то ADATA XPG Lancer 64Gb DDR5 5600MHz станет отличным выбором. Обеспечивая высокую производительность и эффективное охлаждение, этот комплект памяти позволит вам насладиться быстрым и плавным функционированием вашей системы.", "/img/products/64Gb DDR5 5600MHz ADATA XPG Lancer (AX5U5600C3632G-DCLABK) (2x32Gb KIT).webp", "64Gb DDR5 5600MHz ADATA XPG Lancer (AX5U5600C3632G-DCLABK) (2x32Gb KIT)", "{\"Manufacturer\":\"ADATA\",\"ManufacturerCode\":\"AX5U5600C3632G-DCLABK\",\"FormFactor\":\"DIMM\",\"MemoryType\":\"DDR5\",\"MemorySize\":\"64 Гб\",\"ModulesCount\":\"2\",\"ClockSpeed\":\"5600 МГц\"}" },
                    { new Guid("db040c0c-f8f4-48fd-8be4-3040caaa722e"), 5, 3970m, "Оперативная память Netac Shadow II (NTSWD4P32DP-16W) представляет собой высококачественный и надежный комплект из двух модулей по 8 Гб каждый, общим объемом 16 Гб. Этот комплект идеально подходит для апгрейда вашего компьютера, обеспечивая ему стабильную и быструю работу.\r\n\r\nОперативная память выполнена в форм-факторе DIMM и использует технологию DDR4, что обеспечивает высокую скорость передачи данных и энергоэффективность. Тактовая частота модулей составляет 3200 МГц, что позволяет им работать на максимальной производительности. Пропускная способность оперативной памяти составляет 25600 Мб/с, что гарантирует быструю передачу данных и отличную отзывчивость системы.\r\n\r\nХарактеристики оперативной памяти Netac Shadow II включают в себя следующие параметры: CAS Latency (CL) - 16, RAS to CAS Delay (tRCD) - 20, Row Precharge Delay (tRP) - 20, Activate to Precharge Delay (tRAS) - 40. Эти параметры обеспечивают стабильную и эффективную работу оперативной памяти, что в свою очередь повышает производительность вашего компьютера.\r\n\r\nКроме того, оперативная память Netac Shadow II оснащена радиатором для passivной системы охлаждения, который помогает поддерживать низкую температуру модулей во время работы. Это позволяет избежать перегрева и обеспечивает долгий срок службы оперативной памяти.\r\n\r\nОбщий объем памяти 16 Гб позволяет запускать множество приложений одновременно, обеспечивая плавную и быструю работу вашего компьютера. Благодаря высокой производительности и надежности, оперативная память Netac Shadow II является идеальным выбором для тех, кто ценит качество и эффективность.\r\n\r\nПриобретите оперативную память Netac Shadow II (NTSWD4P32DP-16W) и наслаждайтесь быстрой и стабильной работой вашего компьютера без каких-либо задержек и сбоев. Этот комплект станет надежным партнером в вашем повседневном использовании компьютера и поможет вам достичь максимальной производительности.", "/img/products/16Gb DDR4 3200MHz Netac Shadow II (NTSWD4P32DP-16W) (2x8Gb KIT).webp", "16Gb DDR4 3200MHz Netac Shadow II (NTSWD4P32DP-16W) (2x8Gb KIT)", "{\"Manufacturer\":\"Netac\",\"ManufacturerCode\":\"NTSWD4P32DP-16W\",\"FormFactor\":\"DIMM\",\"MemoryType\":\"DDR4\",\"MemorySize\":\"16 Гб\",\"ModulesCount\":\"2\",\"ClockSpeed\":\"3200 МГц\"}" },
                    { new Guid("fa991ad4-510b-47d2-848c-130aadd43838"), 3, 7050m, "M.2 накопитель Kingston NV2 – компактное устройство, способное эффективно справляться с требовательными ресурсоемкими задачами и увеличить отзывчивость системы. Накопитель ориентирован на создание контента с разрешением 4K+ и запуск игр. Благодаря объему 1000 ГБ Kingston NV2 предоставляет достаточно пространства для хранения игр, мультимедийных и прочих файлов. Форм-фактор M.2 гарантирует широкую совместимость с настольными ПК и ноутбуками. Интерфейс подключения PCI-E 4.0 x4 и технология 3D NAND гарантируют скорость в пределах 3500 Мбайт/сек.", "/img/products/SSD-1Tb-Kingston-NV2.webp", "SSD 1Tb Kingston NV2 (SNV2S/1000G)", "{\"Manufacturer\":\"Kingston\",\"ManufacturerCode\":\"SNV2S/1000G\",\"FormFactor\":\"M.2\",\"Capacity\":\"1000 Гб\"}" },
                    { new Guid("ffc4ec07-f264-4930-8892-e22cad344f51"), 5, 8450m, "Оперативная память Patriot Viper Steel RGB (PVSR432G360C0K) - это идеальное решение для тех, кто ищет надежный и быстродействующий компонент для своего ПК. Этот набор памяти состоит из двух модулей по 16 Гб каждый, обеспечивая общий объем впечатляющих 32 Гб.\r\n\r\nСтильный и эффективный дизайн Patriot Viper Steel RGB подарит вашему компьютеру не только высокую производительность, но и стильный внешний вид благодаря встроенной подсветке RGB. Кроме того, радиаторы на модулях памяти обеспечивают эффективное охлаждение, что позволяет поддерживать стабильную работу даже в условиях интенсивной нагрузки.\r\n\r\nС тактовой частотой 3600 МГц и пропускной способностью 28800 Мб/с, память Patriot Viper Steel RGB обеспечивает высокую скорость передачи данных, что делает ее идеальным выбором для геймеров, видеомонтажеров и других пользователей, которым необходимы высокие показатели производительности.\r\n\r\nКроме того, характеристики памяти включают в себя параметры CAS Latency (CL) 20, RAS to CAS Delay (tRCD) 26, Row Precharge Delay (tRP) 26, Activate to Precharge Delay (tRAS) 46, что гарантирует быструю реакцию и эффективную работу в самых требовательных условиях.\r\n\r\nБлагодаря форм-фактору DIMM и поддержке DDR4, установка и настройка памяти Patriot Viper Steel RGB не представляют сложностей, а совместимость с большинством современных материнских плат обеспечивает широкий спектр возможностей для использования.\r\n\r\nНе сомневайтесь в качестве и надежности памяти Patriot Viper Steel RGB - это надежный и высокопроизводительный компонент, который поможет вам справиться с самыми тяжелыми задачами и наслаждаться плавным и быстрым функционированием вашего ПК.", "/img/products/32Gb DDR4 3600MHz Patriot Viper Steel RGB (PVSR432G360C0K) (2x16Gb KIT).webp", "32Gb DDR4 3600MHz Patriot Viper Steel RGB (PVSR432G360C0K) (2x16Gb KIT)", "{\"Manufacturer\":\"Patriot MemoryPatriot Memory\",\"ManufacturerCode\":\"PVSR432G360C0K\",\"FormFactor\":\"DIMM\",\"MemoryType\":\"DDR4\",\"MemorySize\":\"32 Гб\",\"ModulesCount\":\"2\",\"ClockSpeed\":\"3600 МГц\"}" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CanBeDeleted", "Name" },
                values: new object[,]
                {
                    { new Guid("d0febec9-d438-4436-a720-a6026465357d"), false, "Admin" },
                    { new Guid("e56b2b3c-4b78-4e69-a130-e9fd96dbc98f"), false, "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartPosition_CartId",
                table: "CartPosition",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartPosition_ProductId",
                table: "CartPosition",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ComparisonProducts_ProductId",
                table: "ComparisonProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteProducts_ProductId",
                table: "FavoriteProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPosition_OrderId",
                table: "OrderPosition",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderPosition_ProductId",
                table: "OrderPosition",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_InfoId",
                table: "Orders",
                column: "InfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartPosition");

            migrationBuilder.DropTable(
                name: "ComparisonProducts");

            migrationBuilder.DropTable(
                name: "FavoriteProducts");

            migrationBuilder.DropTable(
                name: "OrderPosition");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "UserDeliveryInfo");
        }
    }
}
