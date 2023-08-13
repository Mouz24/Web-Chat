import React, { useState, useEffect } from 'react';
import axios from 'axios';
import TagPanel from './TagsPanel';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Alert, AlertTitle, Box, Button, Container, Grid, Input, Paper, TextField, TextareaAutosize } from '@mui/material';
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
  const [messages, setMessages] = useState<MessageWithTagsDTO[]>([]);
  const [selectedTags, setSelectedTags] = useState<number[]>([]);
  const [availableTags, setAvailableTags] = useState<Tag[]>([]);
  const [newMessage, setNewMessage] = useState('');
  const [hubConnection, setHubConnection] = useState<HubConnection>();
  const [showAlert, setShowAlert] = useState(false);
  const [userId, setUserId] = useState<string | null>(null);
  const [user, SetUser] = useState(false);

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
    fetchMessages();
    fetchTags();

    const hubConnection = new HubConnectionBuilder()
      .withUrl('http://peabody28.com:1030/api')
      .configureLogging(LogLevel.Information)
      .build();

      hubConnection.on('ReceiveMessage', (message) => {
        fetchMessages()
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
      let response;

      if (selectedTags.length === 0) {
        response = await axios.get<MessageWithTagsDTO[]>('http://peabody28.com:1030/api/messages');
      } else {
        const idsQueryParam = selectedTags.map(tag => `ids=${tag}`).join('&');
        response = await axios.get<MessageWithTagsDTO[]>(`http://peabody28.com:1030/api/messages?${idsQueryParam}`);
      }

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

  const handleTagToggle = (tagId: number) => {
    if (selectedTags.includes(tagId)) {
      setSelectedTags(selectedTags.filter((id) => id !== tagId));
    } else {
      setSelectedTags([...selectedTags, tagId]);
    }
    fetchTags();
  };

  const handleSendMessage = async () => {
    if (newMessage.trim() === "") {
      setShowAlert(true);
      return;
    }

    try {
      await hubConnection?.invoke('SendMessage', {
        Text: newMessage,
        Ids: selectedTags,
        SenderId: userId
      });

      setNewMessage('');
    } catch (error) {
      console.error('Error sending message:', error);
    }
  };

  return (
    <Container className="chat-window">
      <Grid container spacing={3}>
        <Grid item xs={4}>
          <Paper elevation={3} className="tag-panel">
            <TagPanel selectedTags={selectedTags} handleTagToggle={handleTagToggle} />
          </Paper>
        </Grid>
        <Grid item xs={8}>
          <Paper elevation={3} className="message-list">
            {messages.map((message) => (
              <div key={message.id} 
              className={`message ${message.message.senderId === userId ? 'sent' : 'received'}`}>
                {message.message && message.message.text && (
                  <p>{message.message.text}</p>
                )}
                {message.message && message.message.timestamp && (
                  <p className="timestamp">
                    {new Date(message.message.timestamp).toLocaleString('en-US', {
                      hour: '2-digit',
                      minute: '2-digit'
                    })}
                  </p>
                )}
                {message.tag && message.tag.length > 0 && (
                  <div className="tags">
                    {message.tag.map((tag) => (
                     tag && tag.text ? (
                      <div key={tag.id} className="tag">
                        {`#${tag.text}`}
                      </div>
                    ) : null
                  ))}
                  </div>
                )}
              </div>
            ))}
          </Paper>
          <Paper elevation={3} className="message-input">
          <TextareaAutosize
            minRows={1}
            maxRows={6}
            value={newMessage}
            onChange={(e) => setNewMessage(e.target.value)}
            placeholder="Type a message..."
            style={{
              width: '100%',
              backgroundColor: '#f7f7f7',
              border: 'none',
              borderRadius: '8px',
              padding: '5px',
              fontFamily: 'Helvetica, Arial, sans-serif',
              fontSize: '14px',
              lineHeight: '1.5',
              resize: 'none',
              outline: 'none',
            }}
          />
            <Button variant="contained" size='small' endIcon={<SendIcon/>} onClick={handleSendMessage} sx={{fontFamily: 'cursive'}}>
              Send
            </Button>
          </Paper>
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
