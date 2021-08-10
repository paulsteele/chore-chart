using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Threading.Tasks;
using hub.Client.Services.Web;
using hub.Shared.Bases;
using hub.Shared.Models.Todo;

namespace hub.Client.ViewModels.Todo
{
    public interface ITodoAllViewModel : INotifyStateChanged
    {
        TodoModel PendingTodoModel { get; }
        List<TodoModel> Todos { get; }

        public Task Save();
    }
    
    public class TodoAllViewModel : BaseNotifyStateChanged, ITodoAllViewModel
    {
        private readonly AuthedHttpClient _httpClient;

        public TodoAllViewModel(AuthedHttpClient httpClient)
        {
            _httpClient = httpClient;

            PendingTodoModel = new TodoModel();
            Todos = new List<TodoModel>();
            LoadTodos().ConfigureAwait(false);
        }

        private async Task LoadTodos()
        {
            await _httpClient.Init();
            
            var responseMessage = await _httpClient.GetFromJsonAsync<TodoModel[]>("todos");
            if (responseMessage != null)
            {
                Todos.InsertRange(0, responseMessage);
                NotifyStateChanged();
            }
        }

        public TodoModel PendingTodoModel { get; private set; }
        
        public List<TodoModel> Todos { get; private set; }
        public async Task Save()
        {
            await _httpClient.Init();

            var responseMessage = await _httpClient.PutAsJsonAsync("todos", PendingTodoModel);
            if (responseMessage.IsSuccessStatusCode)
            {
                PendingTodoModel = new TodoModel();
                var todo = await responseMessage.Content.ReadFromJsonAsync<TodoModel>();
                Todos.Insert(0, todo);
                NotifyStateChanged();
            }
        }
    }
}