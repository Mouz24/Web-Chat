import React, { useState, useEffect } from 'react';
import axios from 'axios';
import TagPanel from './TagsPanel';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Alert, AlertTitle, Autocomplete, Box, Button, Container, Grid, Input, Paper, TextField, TextareaAutosize } from '@mui/material';
import SendIcon from '@mui/icons-material/Send';
import './ChatPanel.css';
import {v4 as uuidv4} from 'uuid';

function generateGuid(): string {
  return uuidv4();
}

interface Message {
  id: string;
  text: string;
  timestamp: string;
  senderId: string;
}

interface Tag {
  id: number;
  text: string;
}

interface MessageWithTagsDTO {
  id: string;
  messageId: string;
  message: Message;
  tagId: number | null;
  tag: Tag[] | null;
}

const ChatWindow: React.FC = () => {
  const [messages, setMessages] = useState<Message[]>([]);
  const [selectedTags, setSelectedTags] = useState<Tag[]>([]);
  const [availableTags, setAvailableTags] = useState<Tag[]>([]);
  const [newMessage, setNewMessage] = useState('');
  const [hubConnection, setHubConnection] = useState<HubConnection>();
  const [showAlert, setShowAlert] = useState(false);
  const [userId, setUserId] = useState<string | null>(null);
  const [user, SetUser] = useState(false);
  const [showTagList, setShowTagList] = useState(false);
  
  useEffect(() => {
    const storedUserId = localStorage.getItem('chatUserId');
    if (storedUserId) {
      setUserId(storedUserId);
    } else {
      const newUserId = generateGuid();
      setUserId(newUserId);
      localStorage.setItem('chatUserId', newUserId);
    }
  }, []);

  useEffect(() => {
    fetchTags();
    fetchMessages();

    const hubConnection = new HubConnectionBuilder()
      .withUrl('http://peabody28.com:1030/api')
      .configureLogging(LogLevel.Information)
      .build();

      hubConnection.on('ReceiveMessage', (message) => {
        fetchTags();
        fetchMessages();
      });
  
    hubConnection
      .start()
      .then(() => {
        console.log('SignalR connection established.');
        setHubConnection(hubConnection);
      })
      .catch((error) => {
        console.error('Error establishing SignalR connection:', error);
      });
  }, [selectedTags, newMessage, userId]);

  const fetchMessages = async () => {
    try {
      const idsQueryParam = selectedTags.map(tag => `tagids=${tag.id}`).join('&');
      const response = await axios.get<Message[]>(`http://peabody28.com:1030/api/messages?${idsQueryParam}`);
      
      setMessages(response.data);
      console.log(messages);
    } catch (error) {
      console.error('Error fetching messages:', error);
    }
  };

  const fetchTags = async () => {
    try {
      const response = await axios.get<Tag[]>('http://peabody28.com:1030/api/tags');
      setAvailableTags(response.data);
    } catch (error) {
      console.error('Error fetching tags:', error);
    }
  };

  const handleTagToggle = (tag: Tag) => {
    if (selectedTags.includes(tag)) {
      setSelectedTags(selectedTags.filter((id) => id !== tag));
    } else {
      setSelectedTags([...selectedTags, tag]);
    }

    setShowTagList(false);
  };

  const handleSendMessage = async () => {
    if (newMessage.trim() === "") {
      setShowAlert(true);
      return;
    }
  
    try {
      const tagsInMessage = newMessage.match(/#(\w+)/g) || [];
      
      const newTags = tagsInMessage
        .filter(tag => !availableTags.some(existingTag => existingTag.text === tag.substring(1)))
        .map(tag => tag.substring(1));
  
      for (const newTagText of newTags) {
        try {
          const response = await axios.post<Tag>('http://peabody28.com:1030/api/tags', { text: newTagText });
          const newTag = response.data;
  
          setAvailableTags(prevTags => [...prevTags, newTag]);
        } catch (error) {
          console.error(`Error creating new tag "${newTagText}":`, error);
        }
      }
      await hubConnection?.invoke('SendMessage', {
        Text: newMessage,
        SenderId: userId
      });
  
      setNewMessage('');
    } catch (error) {
      console.error('Error sending message:', error);
    }
  };
  

  const handleTagInput = (event: React.ChangeEvent<HTMLInputElement>) => {
    const message = event.target.value;
    setShowTagList(message.endsWith("#"));
    setNewMessage(message);
  };

  const handleTagAppend = async (tagText: string) => {
    setNewMessage(newMessage + `${tagText}`);
  };

  return (
    <Container className="chat-window">
      <Grid container spacing={3}>
        <Grid item xs={4}>
          <Paper elevation={3} className="tag-panel">
            <TagPanel selectedTags={selectedTags} handleTagToggle={handleTagToggle} availablTags={availableTags} />
          </Paper>
        </Grid>
        <Grid item xs={8}>
          <Paper elevation={3} className="message-list">
            {messages.map((message) => (
              <div key={message.id} 
              className={`message ${message.senderId === userId ? 'sent' : 'received'}`}>
                {message && message.text && (
                  <p>{message.text}</p>
                )}
                {message && message.timestamp && (
                  <p className="timestamp">
                    {new Date(message.timestamp).toLocaleString('en-US', {
                      hour: '2-digit',
                      minute: '2-digit'
                    })}
                  </p>
                )}
                
              </div>
            ))}
          </Paper>
          <Paper elevation={3} className="message-input" style={{background: '#f8bbd0'}}>
          <TextField
            multiline
            minRows={1}
            maxRows={6}
            sx={{
              backgroundColor: '#f7f7f7',
              borderRadius: '15px',
              fontFamily: 'Helvetica, Arial, sans-serif',
              fontSize: '14px',
              lineHeight: '1.3',
              outline: 'none',
              border: 'none',
              width: '100%',
            }}
            placeholder="Type a message..."
            fullWidth
            value={newMessage}
            onChange={handleTagInput}
          />
            <Button variant="contained" size='small' endIcon={<SendIcon/>} onClick={handleSendMessage} sx={{fontFamily: 'cursive'}}>
              Send
            </Button>
          </Paper>
          {showTagList && (
          <Paper elevation={3} className="tag-list">
            {availableTags.map((tag) => (
              <Button
                key={tag.id}
                onClick={() => handleTagAppend(tag.text)}
                variant='outlined'
                sx={{ margin: '5px' }}
              >
                {tag.text}
              </Button>
            ))}
          </Paper>
        )}

        </Grid>
      </Grid>
      {showAlert && (
        <div className="center-alert">
          <Alert severity="warning" onClose={() => setShowAlert(false)}>
            <AlertTitle>Warning</AlertTitle>
            Cannot send empty or whitespace-only message.
          </Alert>
        </div>
      )}
    </Container>
  );
}

export default ChatWindow;
