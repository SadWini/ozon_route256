# Неделя 5: домашнее задание

## Перед тем как начать
- Как подготовить окружение [см. тут](./docs/01-prepare-environment.md)
- Как накатить миграции на базу данных [см. тут](./docs/02-data-migrations.md)
- **Самое важное!** - полное описание базы данных, схему и описание полей можно найти [тут](./docs/03-db-description.md)

## Основные требования
- решением каждого задания является **один** SQL-запрос
- не допускается менять схему или сами данные, если этого явно не указано в задании
- поля в выборках должны иметь псевдоним (alias), указанный в задании
- решение необходимо привести в блоке каждой задачи ВМЕСТО комментария "ЗДЕСЬ ДОЛЖНО БЫТЬ РЕШЕНИЕ" (прямо в текущем readme.md файле)
- метки времени должны быть приведены в формат _dd.MM.yyyy HH:mm:ss_ (время в БД и выборках в UTC)

## Прочие пожелания
- всем будет удобно, если вы будете придерживаться единого стиля форматирования SQL-команд, как в [этом примере](./docs/04-sql-guidelines.md)

## Задание 1: 100 заданий с наименьшим временем выполнения
Время, затраченное на выполнение задания - это период времени, прошедший с момента перехода задания в статус "В работе" и до перехода в статус "Выполнено".
Нужно вывести 100 заданий с самым быстрым временем выполнения. 
Полученный список заданий должен быть отсортирован от заданий с наименьшим временем выполнения к заданиям с наибольшим временем выполнения.

Замечания:
- Невыполненные задания (не дошедшие до статуса "Выполнено") не учитываются.
- Когда исполнитель берет задание в работу, оно переходит в статус "В работе" (InProgress) и находится там до завершения работы. После чего переходит в статус "Выполнено" (Done).
  В любой момент времени задание может быть безвозвратно отменено - в этом случае оно перейдет в статус "Отменено" (Canceled).
- Нет разницы, выполняется задание или подзадание.
- Выборка должна включать задания за все время.

Выборка должна содержать следующий набор полей:
- номер задания (task_number)
- заголовок задания (task_title)
- название статуса задания (status_name)
- email автора задания (author_email)
- email текущего исполнителя (assignee_email)
- дата и время создания задания (created_at)
- дата и время первого перехода в статус В работе (in_progress_at)
- дата и время выполнения задания (completed_at)
- количество дней, часов, минут и секунд, которые задание находилось в работе - в формате "dd HH:mm:ss" (work_duration)

### Решение
```sql
  select  t.number as task_number
        ,  t.title  as task_title
        ,  ts.name as task_status
        ,  cu.email  as author_email    
        ,  au.email  as assignee_email
        ,  t.created_at as created_at
        ,  tl.at as task_assigned_at
        ,  t.completed_at as completed_at
        ,  date_trunc('second', age(completed_at, tl.at)) as work_duration 
    from tasks t
            join users cu on cu.id = t.created_by_user_id
            join task_statuses ts on ts.id = t.status
                and ts.name = 'Выполнено'
            join task_logs tl on tl.task_id = t.id
                and tl.status = 3 /* InProgress */
       left join users au on au.id = t.assigned_to_user_id
   order by work_duration asc
   limit 100;
```

## Задание 2: выборка для проверки вложенности
Задания могу быть простыми и составными. Составное задание содержит в себе дочерние - так получается иерархия заданий.
Глубина иерархии ограничена Н-уровнями, поэтому перед добавлением подзадачи к текущей задаче нужно понять, может ли пользователь добавить задачу уровнем ниже текущего или нет. Для этого нужно написать выборку для метода проверки перед добавлением подзадания, которая бы вернула уровень вложенности указанного задания и полный путь до него от родительского задания.

Замечания:
- ИД проверяемого задания передаем в sql как параметр _:parent_task_id_
- если задание _Е_ находится на 5м уровне, то путь должен быть "_//A/B/C/D/E_".

Выбора должна содержать:
- только 1 строку
- поле "Уровень задания" (level) - уровень указанного в параметре задания
- поле "Путь" (path)

### Решение
```sql

with recursive TaskHierarchy AS (
       select id
              , parent_task_id 
              , title 
              , 1 AS level 
              , CONCAT('//', title) AS path 
         from tasks
        where id = :parent_task_id 
        union all
       select t.id
              , t.parent_task_id
              , t.title
              , th.level + 1 
              , CONCAT(th.path, '/', t.title) AS path 
         from tasks t
                join TaskHierarchy th on t.parent_task_id = th.id
   )

select
       max(level) as level,
       max(path) as path
  from TaskHierarchy;
```

## Задание 3: самые активные пользователи
Для того, чтобы понимать, насколько активно пользователи используют сервис управления заданиями - необходимо собирать топ 100 активных пользователей по количеству действий, сделанных в системе. Под действием подразумевается любое изменение/создание задачи, а также оставленные комментарии к задаче. Активным пользователем будем считать пользователя, у которого отсутствует блокировка.
Полученный список должен быть отсортирован в порядке убывания по количеству действий, совершенных пользователем. 

Замечания:
- в случае равного количества действий у пользователя, приоритетнее тот, у которого id пользователя меньше

Выборка должна содержать:
- id пользователя (user_id)
- email пользователя (email)
- количество действий, совершенных пользователем в системе (total_events)

### Решение
```sql
with user_actions as (
    select t.created_by_user_id AS user_id
              , count(*) as total_events
      from tasks t
     group by t.created_by_user_id
     union all
    select tl.user_id as user_id
               , count(*) as total_events
      from task_logs tl
     group by tl.user_id
     union all
    select tc.author_user_id as user_id
               , count(*) as total_events
      from task_comments tc
     group by tc.author_user_id
   ),      
    user_event_counts as (
        select user_id
            , sum(total_events) as total_events
          from user_actions
         group by user_id
        )  
          
    select u.id as user_id
           , u.email
           , coalesce(uec.total_events, 0) as total_events
      from users u
 left join user_event_counts uec on u.id = uec.user_id
     where u.blocked_at is null
     order by total_events desc
              , u.id asc
     limit 100;
```

## Дополнительное задание: получить переписку по заданию в формате "вопрос-ответ"
По заданию могут быть оставлены комментарии от Автора и Исполнителя. Автор задания - это пользователь, создавший задание, Исполнитель - это актуальный пользователь, назначенный на задание.
У каждого комментария есть поле _author_user_id_ - это идентификатор пользователя, который оставил комментарий. Если этот идентификатор совпадает с идентификатором Автора задания, то сообщение должно отобразиться **в левой колонке**, следующее за ним сообщение-ответ Исполнителя (если _author_user_id_ равен ИД исполнителя) должно отобразиться **на той же строчке**, что и сообщение Автора, но **в правой колонке**. Считаем, что Автор задания задает Вопросы, а Исполнитель дает на них Ответы.
Выборка должна включать "беседы" по 5 самым новым заданиям и быть отсортирована по порядку отправки комментариев в рамках задания. 

Замечания:
- Актуальный исполнитель - это пользователь на момент выборки указан в поле assiged_to_user_id.
- Если вопроса или ответа нет, в соответствующем поле должен быть NULL.
- Считаем, что все сообщения были оставлены именно текущим исполнителем (без учета возможных переназначений).
- Если комментарий был оставлен не Автором и не Исполнителем, игнорируем его.

Выборка должна содержать следующий набор полей:
- номер задания (task_number)
- email автора задания (author_email)
- email АКТУАЛЬНОГО исполнителя (assignee_email)
- вопрос (question)
- ответ (answer)
- метка времени, когда был задан вопрос (asked_at)
- метка времени, когда был дан ответ (answered_at)

<details>
  <summary>Пример</summary>

Переписка по заданию №1 между author@tt.ru и assgnee@tt.ru:
- 01.01.2023 08:00:00 (автор) "вопрос 1"
- 01.01.2023 09:00:00 (исполнитель) "ответ 1"
- 01.01.2023 09:15:00 (исполнитель) "ответ 2"
- 01.01.2023 09:30:00  (автор) "вопрос 2"

Ожидаемый результат выполнения SQL-запроса:

| task_number | author_email    | assignee_email | question  | answer  | asked_at             | answered_at          |
|-------------|-----------------|----------------|-----------|---------|----------------------|----------------------|
| 1           | author@tt.ru    | assgnee@tt.ru  | вопрос 1  | ответ 1 | 01.01.2023 08:00:00  | 01.01.2023 09:00:00  |
| 1           | author@tt.ru    | assgnee@tt.ru  | вопрос 1  | ответ 2 | 01.01.2023 08:00:00  | 01.01.2023 09:15:00  |
| 1           | author@tt.ru    | assgnee@tt.ru  | вопрос 2  |         | 01.01.2023 09:30:00  |                      |

</details>


### Решение
```sql
   with latest_tasks as (
       select id AS task_id
              , created_at 
              , created_by_user_id as author_id
              , assigned_to_user_id as assignee_id
         from tasks
        order by created_at desc
        limit 5
   ),         
    comments as (
       select c.task_id
              , c.author_user_id
              , c.message 
              , c.at as comment_created_at
         from task_comments c
        where c.author_user_id in (
                select author_id 
                  from latest_tasks
                 union
                select assignee_id 
                  from latest_tasks
                )
        )      
              
       select lt.task_id
              , ua.email as author_email
              , ua_assignee.email as assignee_email
              , max(case when c.author_user_id = lt.author_id then c.message end) as question
              , max(case when c.author_user_id = lt.assignee_id then c.message end) as answer
              , max(case when c.author_user_id = lt.author_id then c.comment_created_at end) as asked_at
              , max(case when c.author_user_id = lt.assignee_id then c.comment_created_at end) as answered_at
         from latest_tasks lt
    left join comments c on lt.task_id = c.task_id
    left join users ua on lt.author_id = ua.id
    left join users ua_assignee on lt.assignee_id = ua_assignee.id
        group by lt.task_id
               , lt.created_at
               , ua.email
               , ua_assignee.email
        order by lt.created_at desc
```
### Дедлайны сдачи и проверки задания:
- 5 октября 23:59 (сдача) / 8 октября, 23:59 (проверка)
