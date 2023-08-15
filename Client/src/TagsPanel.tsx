import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Button, TextField, List, ListItem, ListItemButton, Checkbox, Box, Chip, Alert, AlertTitle } from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import Autocomplete from '@mui/material/Autocomplete';

interface Tag {
  id: number;
  text: string;
}

interface TagPanelProps {
  selectedTags: Tag[];
  handleTagToggle: (tag: Tag) => void;
  availablTags: Tag[];
}

const TagPanel: React.FC<TagPanelProps> = ({ selectedTags, handleTagToggle, availablTags }) => {
  const [tags, setTags] = useState<Tag[]>([]);
  const [selectedAutocompleteTags, setSelectedAutocompleteTags] = useState<Tag[]>([]);
  const [newTag, setNewTag] = useState('');
  const [showAlert, setShowAlert] = useState(false);
  const [alertMessage, setAlertMessage] = useState('');

  useEffect(() => {
    fetchTags();
  }, [availablTags]);

  const fetchTags = async () => {
    try {
      const response = await axios.get<Tag[]>('http://peabody28.com:1030/api/tags');
      setTags(response.data);
    } catch (error) {
      console.error('Error fetching tags:', error);
    }
  };

  const handleTagAdd = async () => {
    if (newTag.trim() === "") {
      setShowAlert(true);
      setAlertMessage('Tag name cannot be empty.');
      return;
    }
  
    const tagToAdd = tags.find(tag => tag.text === newTag);
  
    if (tagToAdd && !selectedAutocompleteTags.some(tag => tag.id === tagToAdd.id)) {
      handleTagToggle(tagToAdd);
    } else {
      try {
        const response = await axios.post('http://peabody28.com:1030/api/tags', { text: newTag });
        const newTagFromApi = response.data;
        setTags(prevTags => [...prevTags, newTagFromApi]);
        handleTagToggle(newTagFromApi);
      } catch (error) {
        console.error('Error adding tag:', error);
      }
    }
  
    setNewTag('');
  };  

  const handleTagDelete = (deletedTag: Tag) => {
    handleTagToggle(deletedTag);
  };

  return (
    <div className="tag-panel">
      <Box>
        <h2>Accessible Tags</h2>
        <Box>
          <Autocomplete
            freeSolo
            options={tags.map(tag => tag.text)}
            value={newTag}
            onInputChange={(event, newValue) => setNewTag(newValue)}
            onKeyPress={(event) => {
              if (event.key === 'Enter') {
                handleTagAdd();
              }
            }}
            renderInput={params => (
              <TextField
                {...params}
                type="text"
                placeholder="Add new tag"
                size="small"
              />
            )}
          />
          <Box sx={{display: 'flex', flexDirection: 'row-reverse'}}>
          <Button variant="outlined" color='success' onClick={handleTagAdd} sx={{marginTop: '8px', color: 'green', 
          '&:hover': {
            backgroundColor: '#2e7d32',
            color: 'white'
          },}}>
            Add
          </Button>
          </Box>
        </Box>
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: '8px', marginTop: '8px'}}>
          {selectedTags.map((tag) => (
            <Chip
              key={tag.id}
              label={tag.text}
              onDelete={() => handleTagDelete(tag)}
              variant='filled'
            />
          ))}
        </Box>
        {showAlert && (
          <Alert sx={{marginTop: '8px'}} severity="warning" onClose={() => setShowAlert(false)}>
            <AlertTitle>Warning</AlertTitle>
            {alertMessage}
          </Alert>
        )}
      </Box>
    </div>
  );
};

export default TagPanel;