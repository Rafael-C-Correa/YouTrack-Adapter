using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TaskItem;

class Program
{
    static async Task Main(string[] args)
    {
        string baseUrl = "xx"; // Substitua pela URL do seu YouTrack
        string username = "xx"; // Substitua pelo seu nome de usuário
        string password = "xx"; // Substitua pela sua senha

        string projectId = "xx"; // Substitua pelo ID do projeto que deseja buscar as tasks

        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Autenticação básica com nome de usuário e senha
            string credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            // Autenticação
            HttpResponseMessage loginResponse = await client.GetAsync("/user/login");
            if (loginResponse.IsSuccessStatusCode)
            {
                // Realiza a requisição para obter as tasks do projeto
                HttpResponseMessage response = await client.GetAsync($"/api/issues?fields=id,type,summary,project(name),State(name)&query=project:+%7BLumagic+on+Linux%7D");

                if (response.IsSuccessStatusCode)
                {
                    // Converte a resposta em JSON para uma lista de tasks
                    string jsonContent = await response.Content.ReadAsStringAsync();
                    List<YouTrackProject> tasks = JsonConvert.DeserializeObject < List <YouTrackProject>>(jsonContent);

                    // Exibe as informações de cada task
                    foreach (YouTrackProject task in tasks)
                    {
                        Console.WriteLine($"ID: {task.Id}");
                        Console.WriteLine($"Project Name: {task.Project.Name}");
                        Console.WriteLine($"Summary: {task.Summary}");
                        Console.WriteLine($"Project Type: {task.state}\n\r");
                    }
                    
                }
                else
                {
                    Console.WriteLine($"Erro na requisição: {response.StatusCode}");
                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Detalhes do erro: {responseContent}");
                }
            }
            else
            {
                Console.WriteLine($"Erro na autenticação: {loginResponse.StatusCode}");
                string responseContent = await loginResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Detalhes do erro: {responseContent}");
            }
        }

        Console.ReadLine();
    }
}

