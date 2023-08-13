import React from 'react';
import TagPanel from './TagsPanel';
import ChatWindow from './ChatPanel';
import { Box } from '@mui/material';

const App: React.FC = () => {
  return (
    <Box>
      <ChatWindow />
    </Box>
  );
};

export default App;
