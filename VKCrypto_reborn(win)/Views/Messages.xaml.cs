using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace VKCrypto_reborn_win_.Views
{
    /// <summary>
    /// Логика взаимодействия для Messages.xaml
    /// </summary>
    public class Chat: INotifyPropertyChanged
    {
        private string name;
        private string surname;
        private string imagepath;
        private long id;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Surname
        {
            get { return surname; }
            set
            {
                surname = value;
                OnPropertyChanged("Surname");
            }
        }
        public string ImagePath
        {
            get { return imagepath; }
            set
            {
                imagepath = value;
                OnPropertyChanged("ImagePath");
            }
        }
        public long Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop="")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }

    public class MessageContent : INotifyPropertyChanged
    {
        private string companion_name;
        private string companion_surname;
        private long companion_id;
        private string companion_photo;
        private string text;
        private List<string> imagesource = new List<string>();

        public string Companion_Name
        {
            get { return companion_name; }
            set
            {
                companion_name = value;
                OnPropertyChanged("Companion_Name");
            }
        }
        public string Companion_Surname
        {
            get { return companion_surname; }
            set
            {
                companion_surname = value;
                OnPropertyChanged("Companion_Surname");
            }
        }
        public long Companion_Id
        {
            get { return companion_id; }
            set
            {
                companion_id = value;
                OnPropertyChanged("Companion_Id");
            }
        }
        public string Companion_Photo
        {
            get { return companion_photo; }
            set
            {
                companion_photo = value;
                OnPropertyChanged("Companion_Photo");
            }
        }
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged("Text");
            }
        }
        public string ImageSource
        {
            get { return imagesource.LastOrDefault(); }
            set
            {
                imagesource.Add(value);
                OnPropertyChanged("ImageSource");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
    public partial class Messages : UserControl
    {
        public ObservableCollection<Chat> Chats { get; set; }
        public ObservableCollection<MessageContent> ChatContent { get; set; }
        public Messages()
        {
            Chats = new ObservableCollection<Chat> { };
            ChatContent = new ObservableCollection<MessageContent> { };            
            InitializeComponent();
            ChatList.ItemsSource = Chats;
            Chat.ItemsSource = ChatContent;
            Task CreateChatListTask = new Task(new Action(Create_ChatList));
            CreateChatListTask.Start();
        }

        private void Create_ChatList()
        {
            VkNet.Model.GetConversationsResult Conversations = Utils.Userapi.Messages.GetConversations(new VkNet.Model.RequestParams.GetConversationsParams
            {
                Count = 200
            });
            foreach (VkNet.Model.ConversationAndLastMessage conversation in Conversations.Items)
            {
                string name="";
                string surname="";
                string imagepath="";
                long id = conversation.Conversation.Peer.Id;
                if (conversation.Conversation.Peer.Type == VkNet.Enums.SafetyEnums.ConversationPeerType.Chat)
                {
                    name = conversation.Conversation.ChatSettings.Title;
                    if (conversation.Conversation.ChatSettings.Photo != null)
                    {
                        imagepath = conversation.Conversation.ChatSettings.Photo.Photo100.AbsoluteUri;
                    }
                }
                if (conversation.Conversation.Peer.Type == VkNet.Enums.SafetyEnums.ConversationPeerType.User)
                {
                    var names = GetUserNames(id);
                    name = names.Item1; surname = names.Item2; imagepath = names.Item3;
                }
                if (conversation.Conversation.Peer.Type == VkNet.Enums.SafetyEnums.ConversationPeerType.Group)
                {
                    var names = GetGroupNames(id);
                    name = names.Item1; surname = names.Item2; imagepath = names.Item3;
                }
                
                using (WebClient webClient = new WebClient())
                {
                    try
                    {
                        webClient.DownloadFile(imagepath, (System.IO.Path.Combine(Environment.CurrentDirectory, "Friendava" + id.ToString() + ".jpg")));
                    }
                    catch {}
                }
                string imagelocalpath = System.IO.Path.Combine(Environment.CurrentDirectory, "Friendava" + id.ToString() + ".jpg");
                Dispatcher.Invoke(new Action(() =>
                {
                    Chats.Add(new Chat { Name = name, Surname = surname, ImagePath = imagelocalpath, Id = id });
                }));
            }
        }
        private void Search_btn_Click(object sender, RoutedEventArgs e)
        {
            IdConv idConvWindow = new IdConv();
            idConvWindow.Show();
        }
        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            var item = ChatList.SelectedItem;
            if (item != null)
            {
                var curchat = item as Chat;
                MessageBox.Show(curchat.Id.ToString());
            }
        }

        private (string, string, string) GetUserNames(long id)
        {

            VkNet.Model.User user = Utils.Userapi.Users.Get(new long[] { id }, VkNet.Enums.Filters.ProfileFields.Photo100)[0];
            if (user == null)
            {
                return ("", "", "");
            }
            else
            {
                return (user.FirstName, user.LastName, user.Photo100.AbsoluteUri);
            }
        }
        private (string, string, string) GetGroupNames(long id)
        {
            VkNet.Model.Group group = Utils.Userapi.Groups.GetById(null, (-id).ToString(), VkNet.Enums.Filters.GroupsFields.All)[0];
            
            if (group == null)
            {
                return ("", "", "");
            }
            else
            {
                return (group.Name, "", group.Photo100.AbsoluteUri);
            }
        }
    }
}
