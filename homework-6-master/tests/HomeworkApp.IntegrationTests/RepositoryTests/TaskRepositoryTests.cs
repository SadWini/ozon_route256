using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HomeworkApp.Dal.Models;
using HomeworkApp.Bll.Converters;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Creators;
using HomeworkApp.IntegrationTests.Fakers;
using HomeworkApp.IntegrationTests.Fixtures;
using Xunit;
using TaskStatus = HomeworkApp.Dal.Enums.TaskStatus;

namespace HomeworkApp.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskRepositoryTests
{
    private readonly ITaskRepository _repository;

    public TaskRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskRepository;
    }

    [Fact]
    public async Task Add_Task_Success()
    {
        // Arrange
        const int count = 5;

        var tasks = TaskEntityV1Faker.Generate(count);

        // Act
        var results = await _repository.Add(tasks, default);

        // Asserts
        results.Should().HaveCount(count);
        results.Should().OnlyContain(x => x > 0);
    }

    [Fact]
    public async Task Get_SingleTask_Success()
    {
        // Arrange
        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId);

        // Act
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);

        // Asserts
        results.Should().HaveCount(1);
        var task = results.Single();

        task.Should().BeEquivalentTo(expectedTask);
    }

    [Fact]
    public async Task AssignTask_Success()
    {
        // Arrange
        var assigneeUserId = Create.RandomId();

        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId)
            .WithAssignedToUserId(assigneeUserId);
        var assign = AssignTaskModelFaker.Generate()
            .First()
            .WithTaskId(expectedTaskId)
            .WithAssignToUserId(assigneeUserId);

        // Act
        await _repository.Assign(assign, default);

        // Asserts
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);

        results.Should().HaveCount(1);
        var task = results.Single();

        expectedTask = expectedTask with {Status = assign.Status};
        task.Should().BeEquivalentTo(expectedTask);
    }
    
    [Fact]
    public async Task GetSubTasksInStatus_ReturnsCorrectSubTasks()
    {
        // Arrange
        var parentTaskId = Create.RandomId(); 
        var statuses = new[] { TaskStatus.InProgress, TaskStatus.Canceled, TaskStatus.Draft};
    
        var parentTask = TaskEntityV1Faker.Generate(1)
            .First()
            .WithId(parentTaskId);

        var subTask1 = TaskEntityV1Faker.Generate(1)
            .First()
            .WithParentTaskId(parentTaskId)
            .WithStatus(TaskStatusConverter.Map(TaskStatus.InProgress)); 

        var subTask2 = TaskEntityV1Faker.Generate(1)
            .First()
            .WithParentTaskId(parentTaskId)
            .WithStatus(TaskStatusConverter.Map(TaskStatus.Canceled)); 
        
        await _repository.Add(new[] { parentTask, subTask1, subTask2}, default);

        // Act
        var results = await _repository.GetSubTasksInStatus(parentTaskId, statuses, default);

        // Asserts
        results.Should().HaveCount(2); 
    }


    [Fact]
    public async Task GetSubTasksInStatus_ReturnsEmpty()
    {
        // Arrange
        var parentTaskId = Create.RandomId(); 
        var statuses = new[] { TaskStatus.InProgress, TaskStatus.Done };
        var parentTask = TaskEntityV1Faker.Generate(1)
            .First()
            .WithId(parentTaskId);

        var subTaskLevel1 = TaskEntityV1Faker.Generate(1)
            .First()
            .WithParentTaskId(parentTaskId)
            .WithStatus(TaskStatusConverter.Map(TaskStatus.Draft)); 
        
        await _repository.Add(new[] { parentTask, subTaskLevel1 }, default);

        // Act
        var results = await _repository.GetSubTasksInStatus(parentTaskId, statuses, default);

        // Asserts
        results.Should().BeEmpty(); 
    }
}
