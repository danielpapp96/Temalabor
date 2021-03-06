# Kanban board
## ASP.NET backend and React.js frontend



A konkrét feladat egy Kanban board létrehozása amin a teendőket prioritásuk és státuszuk szerint oszlopokba rendezve jelenít meg.

A teendők a user által létrehozhatók, szerkezthetők, törölhetők, státuszuk módosítható.

## Főbb funkcók

- Teendő létrehozása megjelenő moduláris ablakban
- Teendő szekeztése megjelnő moduláris ablakban
- Teendő törlése törlés gombra kattintva
- Teendő KÉSZ státuszba állítása gombra kattintva
- A felületen történő módosításaok API hívással backenden mentődik le


## Használt technológiák

Az alkalmazás az alábbi technológiákat használja:

- [ReactJs] - JavaScript alapú frontend framework
- [ASP.NET] - ASP.NET framework felel a backend REST funkcókért
- [MSSQL] - adatbázis 
- [Bootstrap] - design


## Alkalmazás felépítése

Az alkalmazás klasszikusan felbontható frontend-re és backend-re.

### Backend

A backend kód háromrétegű architektúra szerint lett fejlesztve. Ezek a rétegek a :
- Data Access Layer
- Business Logic Layer
- Presentation Layer

#### Data Access Layer Kanban.Dal

![image](https://user-images.githubusercontent.com/39564586/117572803-e535dd80-b0d4-11eb-8715-53788f129e78.png)

Három fő részből áll a Dtos, Models és Repositories mappákból, ezek feladatukat tekintve lettek így elszeparálva.

A Dtos mappa tárolja a Data Transfer Objecteket, ilyen típusú információkat szolgáltat a backend a frontend számára.

Az alkalmazás Dto-i:

#### CreateTasDto
Egy új teendőlétrehozásakor ilyen objektumot vár a controller.
```
public class CreateTaskDto
    {
       public string Title { get; set; }
        public string Description { get; set; }
    }
```
Válaszul pedig egy TasdDto-t küld vissza, ahol már van kitöltött Id property:
```
public class TaskDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string State { get; set; }

        public string CreatedAt { get; set; }
    }
```

#### CreateTaskColumnDto
Egy teendőket tároló oszlop létrehozáskor ilyen objektumot vár a controller

```
    public class CreateTaskColumnDto
    {
        public string State { get; set; }
    }
```
A válasz egy TaskColumnDto:
```
    public class TaskColumnDto
    {
        public int ID { get; set; }
        public string State { get; set; }
        public IEnumerable<TaskDto> Tasks { get; set; }
    }
```
A Models mappában az alkalmazás entitánsai vannak, ők írják le az adatbázis modelt és a táblák közötti kapcsolatot.

Az alkalmazás entitánsai:

#### TaskModel
Egy teendő adatmodeljét valósítja meg:
```
public class TaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public State? State { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? DeadLine { get; set; }


        public int? TaskColumnID { get; set; }

        public TaskColumn? TaskColumn { get; set; }
    }
```

#### TaskColumn
A teendőket és státuszokat tároló adatmodel:
```
public class TaskColumn
    {
        public int ID { get; set; }
        public State State { get; set; }
        public List<TaskModel> Tasks { get; set; }
    }
```

A Repositories mappában Repository Pattern alapján helyezkednek az adatelérési réteg interfészei, ezeket éri el a Bll réteg adatelérés céljából.

A két repository interfész:

#### ITaskRepository
```
public interface ITaskRepository
    {
        IEnumerable<TaskDto> GetAll();
        TaskDto GetById(int id);
        TaskDto CreateNewTask(CreateTaskDto dto);
        TaskDto UpdateTask(TaskDto dto);
        TaskDto DeleteTask(TaskDto dto);
        bool TaskIsExist(int id);
    }
```

#### IColumnRepository
```
    public interface IColumnRepository
    {
        IEnumerable<TaskColumnDto> GetTaskColumns();
        TaskColumnDto GetById(int id);
        TaskColumnDto CreateNewColumn(CreateTaskColumnDto dto);
        TaskColumnDto AddTaskToColumn(int id, TaskDto dto);
        TaskColumnDto DeleteColumn(TaskColumnDto dto);
        TaskDto MakeTaskDone(int id);
        bool ColumnIsExist(int id);
    }
```

### Business Logic Layer Kanban.Bll

![image](https://user-images.githubusercontent.com/39564586/117572818-fb439e00-b0d4-11eb-8d9e-08fb48be281e.png)

Egy mappában található az alakmazás egyetlen service osztálya és annak interface. A megjelenítési réteg kéréseit szolgálja ki a Dal rétegtől lekért és transformált adatokkal.
Az ITaskService és annak implementálása a TaskService delegálja az adatelérési feladatokat a Dal repository-iknak, ez a metódus neveken is látszik, hogy megegyeznek.

#### ITaskService
```
    public interface ITaskService
    {
        IEnumerable<TaskDto> GetAll();
        TaskDto GetById(int id);
        TaskDto CreateNewTask(CreateTaskDto dto);
        TaskDto UpdateTask(TaskDto dto);
        TaskDto DeleteTask(int id);
        IEnumerable<TaskColumnDto> GetTaskColumns();
        TaskColumnDto CreateNewColumn(CreateTaskColumnDto dto);
        TaskColumnDto AddTaskToColumn(int id, TaskDto dto);
        TaskColumnDto DeleteColumn(int id);
        TaskDto MakeTaskDone(int id);
        bool ColumnIsExist(int id);
        bool TaskIsExist(int id);
    }
```

### Presentation Layer Kanban.Web

A megjelenítési rétegban találhatók a controller osztályok, amik a frontendről jövő http kéréseket fogadják és adják vissza a megfelelő válaszokat.

![image](https://user-images.githubusercontent.com/39564586/117572835-0d254100-b0d5-11eb-8e7e-0894dbb812d4.png)


#### ColumnController
Kezeli az alkalmazás felületéről érkező, oszlopokat érintő kérések nagy részét.

Get() : lekéri az összes TaskColumn entitáns és azokat Dto-ként adja vissza 200-as Ok státusszal.
```
 [HttpGet]
        public ActionResult<IEnumerable<TaskColumnDto>> Get()
        {
            return Ok(taskService.GetTaskColumns());
        }
```
CreateNewColumn() : létrehoz egy új oszlopot, paraméterül vár egy CreateTaskColumnDto-t. Ha a dto nem érkezik be 400-as BadRequest választ ad vissza, sikeres müvelet esetén 200-as Ok státusszal vissza küldi a lementett oszlop dto-ját.
```
        [HttpPost]
        public ActionResult<TaskColumnDto> CreateNewColumn([FromBody] CreateTaskColumnDto dto)
        {
            if(dto == null)
            {
                return BadRequest();
            }

            return Ok(taskService.CreateNewColumn(dto));
        }
```
AddTaskToColumn() : paraméterül várja az oszlop id-ját amihez hozzá szeretnénk adni az adott teendőt. Az adott teendő TaskDto-ként kérkezik a kérés body-ban. A validáció itt az oszlop és a teendő létezését vizsgálja, ha nem létezik akár az egyik akkor 404-es NotFound választ küld vissza. Sikeres művelet után 200-as Ok válaszba küldi a módosult TaskColumn-t.
```
        [HttpPost("{id}")]
        public ActionResult<TaskColumnDto> AddTaskToColumn(int id, [FromBody] TaskDto taskDto)
        {
            if(!taskService.TaskIsExist(taskDto.Id) || !taskService.ColumnIsExist(id))
            {
                return NotFound();
            }

            return Ok(taskService.AddTaskToColumn(id, taskDto));
        }
```
DeleteColumn(): paramétere az id ami alapján törölni szeretnénk az oszlopot, ha id alapján nem találja a program 404 NotFound választ küld vissza, sikeres művelet esetén pedig 200-as Ok-val a törölt oszlopot.
```
[HttpDelete("{id}")]
        public ActionResult<TaskColumnDto> DeleteColumn(int id)
        {
            if (!taskService.ColumnIsExist(id))
            {
                return NotFound($"Column with id: {id} not found!");
            }

            return Ok(taskService.DeleteColumn(id));
        }
```
#### TaskController
Kezeli az alkalmazás felületéről érkező, teendőket érintő kérések nagy részét.

Get() : lekéri az összes Task entitáns és azokat Dto-ként adja vissza 200-as Ok státusszal.
```
        [HttpGet]
        public ActionResult<IEnumerable<TaskDto>> Get()
        {
            return Ok(taskService.GetAll());
        }
```

GetById() : paraméterben kapott id-val lekéri a teendőt, nem létező teendő esetén 404-es NotFound választ küld vissza, sikeres lekérés esetén 200-as Ok válaszban a lekért teendő dto-ként.
```
        [HttpGet("{id}")]
        public ActionResult<TaskDto> GetById(int id)
        {
            var task = taskService.GetById(id);

            if(task == null)
            {
                return NotFound();
            }

            return Ok(task);
        }
```

CreateNewTask(): a kérés body-ban kap egy CreateTaskDto-t, ha a dto üres 400-as BadRequest, sikeres mentés után 200-as Ok státusszal visszaküldi a lementett teendőt dto-ként.
```
        [HttpPost]
        public ActionResult<TaskDto> CreateNewTask([FromBody] CreateTaskDto dto)
        {
            if(dto == null)
            {
                return BadRequest();
            }
            return Ok(taskService.CreateNewTask(dto));
        }
```

UpdateTask(): a kérés body-ban érkező dto alapján frissíti a dto id alapján lekért teendőt. Ha a dto null 400-as BadRequest, sikeres update esetén az updatelt teendőt küldi vissza 200-as Ok státusszal
```
        [HttpPut]
        public ActionResult<TaskDto> UpdateTask([FromBody] TaskDto dto)
        {
            var task = taskService.GetById(dto.Id);

            if(task == null)
            {
                return NotFound();
            }

            return Ok(taskService.UpdateTask(dto));
        }
```
DeleteTask(): a paraméterban kapott id alapján kitörli a teendőt. Ha id alapján nem találja akkor 404-es NotFound, sikeres törlés után 200-as ok válaszban a kitörölt teendő dto-t küldi vissza.

```
        [HttpDelete("{id}")]
        public ActionResult<TaskDto> DeleteTask(int id)
        {
            if (!taskService.TaskIsExist(id))
            {
                return NotFound();
            }

            return Ok(taskService.DeleteTask(id));
        }
```

MakeTaskDone(): paraméterben kapott id alapján DONE státuszra állítja a teendőt. Ha id alapján nem találja akkor 404-es NotFound, sikeres művelet után 200-as ok válaszban a módosított teendő dto-t küldi vissza.
```
        [HttpPost("{id}")]
        public ActionResult<TaskDto> MakeTaskDone(int id)
        {
            if (!taskService.TaskIsExist(id))
            {
                return NotFound();
            }

            return Ok(taskService.MakeTaskDone(id));
        }
```


