import { useState } from 'react';
import { StyleSheet, TextInput, View, Button, Text, ActivityIndicator } from 'react-native';

export default function HomeScreen() {
  const [text, setText] = useState('');
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');

  const submitEntry = async () => {
    if (!text.trim()) return;

    setLoading(true);
    setMessage('');

    try {
      const response = await fetch('https://localhost:5002/graphql', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Accept': 'application/json',
        },
        body: JSON.stringify({
          query: `
            mutation SubmitEntry($input: DailyEntryInput!) {
              submitDailyEntry(input: $input) {
                recordId
                success
              }
            }
          `,
          variables: {
            input: {
              rawInput: text
            }
          }
        })
      });

      const result = await response.json();
      
      if (result.errors) {
        setMessage('Failed to submit entry.');
        console.error(result.errors);
      } else {
        setMessage('Entry submitted successfully!');
        setText('');
      }
    } catch (err) {
      setMessage('Error connecting to backend.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <View style={styles.container}>
      <Text style={styles.title}>Daily Exposome Capture</Text>
      <Text style={styles.subtitle}>How was your day? Log your meals, mood, location and meds.</Text>

      <TextInput
        style={styles.input}
        multiline
        placeholder="e.g., Ate chicken tajine for lunch, walked the dog around 3 PM, felt a bit tired in the late afternoon"
        value={text}
        onChangeText={setText}
      />

      <View style={styles.buttonContainer}>
        <Button 
          title="Submit Daily Entry" 
          onPress={submitEntry} 
          disabled={loading || !text.trim()} 
        />
      </View>

      {loading && <ActivityIndicator style={styles.spinner} />}
      {message ? <Text style={styles.message}>{message}</Text> : null}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 24,
    backgroundColor: '#fff',
    justifyContent: 'center',
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 8,
    textAlign: 'center',
  },
  subtitle: {
    fontSize: 16,
    color: '#666',
    marginBottom: 24,
    textAlign: 'center',
  },
  input: {
    height: 120,
    borderColor: '#ccc',
    borderWidth: 1,
    borderRadius: 8,
    padding: 12,
    fontSize: 16,
    textAlignVertical: 'top',
  },
  buttonContainer: {
    marginTop: 16,
  },
  spinner: {
    marginTop: 16,
  },
  message: {
    marginTop: 16,
    textAlign: 'center',
    fontSize: 16,
    color: '#333',
  }
});
