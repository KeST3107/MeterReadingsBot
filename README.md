# MeterReadingsBot
[![MeterReadingsBot](https://github.com/KeST3107/MeterReadingsBot/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/KeST3107/MeterReadingsBot/actions/workflows/dotnet.yml)
### TODO List:
### Стараться каждую таску делать отдельной веткой и сливать ее в dev => master
Сделать корректное отображение часового пояса в бд  
Если таска по получению клиента долго выполняется, то приложение должно возвращать ответ "не можем получить данные попробуйте еще раз" DONE  
Логирование в нужных местах + блоки try catch DONE  
Вынести в ресурсы приложения, все что на русском передается в сообщения  
Настроить массовую рассылку (1. по таймингу указанным в сеттингах с фотками, 2. По сообщению от мастер юзера(фильтрация по айди + собственные команды)) DONE  
Покрыть тестами NUnit + Moq в отдельной сборке  
Проверить корректность Dockerfile  
Посмотреть где можно будет развернуть (Github, reg.ru и тд)  
Попробовать настроить pipeline на мастер (в идеале на дев для реальных тестов приложения)  
Добавить динамическое построение маршрута сообщений, смотря какие счетчики стоят у клиента, строить динамический запрос ответа на то что заполнит клиент.  
Добавить возврат в начальною меню со всех этапов сообщений. DONE  
Добавить прогнозируемый расчет показаний и предлагать пользователю с конфигурируемым ожиданием  
Добавить учет того что пользователь уже отправлял показания, а также не давать передавать по одному лицевому счету дважды за месяц DONE  
Сделать TelegramException и их обработчик, чтобы они улетали напрямую в телегу DONE  
Подумать как можно сделать UnitOfWork, чтобы уйти от проблемы сохранения в репозитории при каждом взаимодействии.  
Отрефакторить код взаимодействия с пользователем внутри сообщений, вынести в общий класс, либо найти готовое решение.  
