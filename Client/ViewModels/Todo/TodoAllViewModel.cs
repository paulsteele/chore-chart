using System.Collections.Generic;
using System.Linq;
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
        List<TodoCompletionPairing> Todos { get; }

        public Task Save();
        public Task DeleteItem(TodoModel item);
        public Task ToggleItem(TodoModel item, bool status);
    }
    
    public class TodoAllViewModel : BaseNotifyStateChanged, ITodoAllViewModel
    {
        private readonly AuthedHttpClient _httpClient;

        public TodoAllViewModel(AuthedHttpClient httpClient)
        {
            _httpClient = httpClient;

            PendingTodoModel = new TodoModel();
            Todos = new List<TodoCompletionPairing>();
            LoadTodos().ConfigureAwait(false);
        }

        private async Task LoadTodos()
        {
            await _httpClient.Init();
            
            var responseMessage = await _httpClient.GetFromJsonAsync<TodoCompletionPairing[]>("todos");
            if (responseMessage != null)
            {
                Todos.InsertRange(0, responseMessage);
                NotifyStateChanged();
            }
        }

        public TodoModel PendingTodoModel { get; private set; }
        
        public List<TodoCompletionPairing> Todos { get; private set; }
        public async Task Save()
        {
            await _httpClient.Init();

            var responseMessage = await _httpClient.PutAsJsonAsync("todos", PendingTodoModel);
            if (responseMessage.IsSuccessStatusCode)
            {
                PendingTodoModel = new TodoModel();
                var todo = await responseMessage.Content.ReadFromJsonAsync<TodoModel>();
                Todos.Insert(0, new TodoCompletionPairing {TodoModel = todo});
                NotifyStateChanged();
            }
        }

        public async Task DeleteItem(TodoModel item)
        {
            await _httpClient.Init();
            var responseMessage = await _httpClient.DeleteAsync($"todos/{item.Id}");

            if (responseMessage.IsSuccessStatusCode)
            {
                Todos.RemoveAll(i => i.TodoModel == item);
                NotifyStateChanged();
            }
        }

        public async Task ToggleItem(TodoModel item, bool status)
        {
            await _httpClient.Init();
            var pairing = Todos.FirstOrDefault(tp => tp.TodoModel == item);
            if (pairing != null)
            {
                if (status)
                {
                    var pendingCompletion = new TodoCompletion {TodoModel = item};
                    var responseMessage = await _httpClient.PutAsJsonAsync("todos/completions", pendingCompletion);
                    if (responseMessage.IsSuccessStatusCode)
                    {
                        var completion = await responseMessage.Content.ReadFromJsonAsync<TodoCompletion>();
                        pairing.TodoCompletion = completion;
                    }
                }
                else
                {
                    var responseMessage = await _httpClient.DeleteAsync($"todos/completions/{pairing.TodoCompletion.Id}");

                    if (responseMessage.IsSuccessStatusCode)
                    {
                        pairing.TodoCompletion = null;
                    }
                }
            }
            
            NotifyStateChanged();
        }
    }
}