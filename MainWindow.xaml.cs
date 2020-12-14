//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Web;
using Newtonsoft.Json;

namespace Lab6RestClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client;
        private string token;
        private string password;
        private string username;
        public MainWindow()
        {
            InitializeComponent();
            client = new HttpClient();          
            client.BaseAddress = new Uri("http://localhost:57667/");
        }
        private async void GetPeople()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (BasicCheckBox.IsChecked ?? false)
            {
                var authStr = string.Format("{0}:{1}", username, password);
                var authBytes = Encoding.UTF8.GetBytes(authStr);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
            }
            else
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var responseMessage = await client.GetAsync("api/people");
            if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var people = JsonConvert.DeserializeObject<List<Person>>(await responseMessage.Content.ReadAsStringAsync());
                PeopleListBox.ItemsSource = people;
            }
            else
            {
                MessageBox.Show(await responseMessage.Content.ReadAsStringAsync(), "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void GetPeopleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetPeople();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void GetPersonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (BasicCheckBox.IsChecked ?? false)
                {
                    var authStr = string.Format("{0}:{1}", username, password);
                    var authBytes = Encoding.UTF8.GetBytes(authStr);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
                }
                else
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var responseMessage = await client.GetAsync("api/people/" + IdTB.Text);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var person = JsonConvert.DeserializeObject<Person>(await responseMessage.Content.ReadAsStringAsync());
                    List<Person> people = new List<Person> { person };
                    PeopleListBox.ItemsSource = people;
                }
                else
                {
                    MessageBox.Show(await responseMessage.Content.ReadAsStringAsync(), "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void DelPersonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PeopleListBox.SelectedItem != null)
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (BasicCheckBox.IsChecked ?? false)
                    {
                        var authStr = string.Format("{0}:{1}", username, password);
                        var authBytes = Encoding.UTF8.GetBytes(authStr);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
                    }
                    else
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    var responseMessage = await client.DeleteAsync("api/people/" + (PeopleListBox.SelectedItem as Person).Id);
                    if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Pomyślnie usunięto " + (PeopleListBox.SelectedItem as Person).ToString(), "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                        GetPeople();
                    }
                    else
                    {
                        MessageBox.Show(await responseMessage.Content.ReadAsStringAsync(), "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void AddPersonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(int.TryParse(YearTB.Text, out int n))
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (BasicCheckBox.IsChecked ?? false)
                    {
                        var authStr = string.Format("{0}:{1}", username, password);
                        var authBytes = Encoding.UTF8.GetBytes(authStr);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
                    }
                    else
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    Person person = new Person();
                    person.Name = NameTB.Text;
                    person.Surname = SurnameTB.Text;
                    person.City = CityTB.Text;
                    person.Year = n;

                    var json = JsonConvert.SerializeObject(person);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var responseMessage = await client.PostAsync("api/people?city=" + cityCheckBox.IsChecked.ToString(), content);
                    if (responseMessage.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        MessageBox.Show("Pomyślnie dodano " + person.ToString(), "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                        GetPeople();
                    }
                    else if (responseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        MessageBox.Show(await responseMessage.Content.ReadAsStringAsync(), "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(await responseMessage.Content.ReadAsStringAsync(), "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {                     
                    MessageBox.Show("Niepoprawna wartość w polu Rok","",MessageBoxButton.OK,MessageBoxImage.Error);                    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void EditPersonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(YearTB.Text, out int n))
                {
                    if(PeopleListBox.SelectedItem != null)
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        if (BasicCheckBox.IsChecked ?? false)
                        {
                            var authStr = string.Format("{0}:{1}", username, password);
                            var authBytes = Encoding.UTF8.GetBytes(authStr);
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
                        }
                        else
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        }

                        Person person = new Person();
                        person.Name = NameTB.Text;
                        person.Surname = SurnameTB.Text;
                        person.City = CityTB.Text;
                        person.Year = n;

                        var json = JsonConvert.SerializeObject(person);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var responseMessage = await client.PutAsync("api/people/" + (PeopleListBox.SelectedItem as Person).Id, content);
                        if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            MessageBox.Show("Pomyślnie zmieniono " + (PeopleListBox.SelectedItem as Person).ToString(), "OK", MessageBoxButton.OK, MessageBoxImage.Information);
                            GetPeople();
                        }
                        else
                        {
                            MessageBox.Show(await responseMessage.Content.ReadAsStringAsync(), "", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }    
                }
                else
                {
                    MessageBox.Show("Niepoprawna wartość w polu Rok", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void FindPersonButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                if (BasicCheckBox.IsChecked ?? false)
                {
                    var authStr = string.Format("{0}:{1}", username, password); 
                    var authBytes = Encoding.UTF8.GetBytes(authStr);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));
                }
                else
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var query = HttpUtility.ParseQueryString(string.Empty);
                query["name"] = NameTB.Text;
                query["surname"] = SurnameTB.Text;
                query["city"] = CityTB.Text;
                query["year"] = YearTB.Text;
                query["lowercase"] = lowercaseCheckBox.IsChecked.ToString();
                query["contains"] = containsCheckBox.IsChecked.ToString();
                string queryString = query.ToString();


                var responseMessage = await client.GetAsync("api/people/find?" + queryString);
                //var responseMessage = await client.GetAsync("api/people/find?" + "name=&surname=&city=Bia%u0142a&year=&lowercase=True&contains=True");
                
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var people = JsonConvert.DeserializeObject<List<Person>>(await responseMessage.Content.ReadAsStringAsync());
                    PeopleListBox.ItemsSource = people;
                }
                else
                {
                    MessageBox.Show(await responseMessage.Content.ReadAsStringAsync(), "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (BasicCheckBox.IsChecked ?? false)
            {
                username = UsernameTextBox.Text;
                password = PasswordBox.Password;
                MessageBox.Show("Zapisano dane do autoryzacji Basic Auth", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                AuthenticateRequest user = new AuthenticateRequest();
                user.Username = UsernameTextBox.Text;
                user.Password = PasswordBox.Password;

                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var responseMessage = await client.PostAsync("api/user", content);
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    AuthenticateResponse response = JsonConvert.DeserializeObject<AuthenticateResponse>(await responseMessage.Content.ReadAsStringAsync());
                    token = response.Token;
                    MessageBox.Show("Pomyślnie zalogowano \n" + "Witaj " + response.Username, "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(await responseMessage.Content.ReadAsStringAsync(), "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
