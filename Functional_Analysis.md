# Functional Analysis: Daily Exposome & Health Tracker

## 1. Executive Summary
The system is a proactive, frictionless personal health and lifestyle tracking application. It reverses the standard "manual entry" paradigm by prompting the user daily via a push notification, capturing natural language or voice input, and utilizing AI to structure and enrich the data into a comprehensive personal database (Exposome).

## 2. Core Workflows
### 2.1 The Daily Capture
* **Trigger:** A scheduled daily push notification to the user's mobile device.
* **Interaction:** The user opens the React Native app and records a short, natural language summary of their day (e.g., "Ate chicken tajine for lunch, walked the dog around 3 PM, felt a bit tired in the late afternoon").
* **Initial Processing:** The app transcribes the audio (if voice is used) and submits the raw text payload via a GraphQL mutation.

### 2.2 Background Data Enrichment (Asynchronous)
* **Entity Extraction:** The system parses the raw input into core categories: Diet, Medication, Mood, and Context/Location.
* **Third-Party Integrations:**
  * **Weather & Environment:** Fetches solar data, PM2.5, and local weather based on location coordinates (via Open-Meteo/OpenWeatherMap).
  * **Nutrition:** Resolves natural language food entries into macronutrients and ingredients (via Edamam or Open Food Facts).
  * **Pharmacology:** Maps medications to active chemical compounds (via RxNorm).
* **Storage:** The enriched dataset is assembled and persisted as schemaless JSON blocks within the user's daily record.

## 3. Data Entities (Logical Model)
* **User:** Profile, notification preferences, timezone settings.
* **DailyRecord:** The aggregate root containing the raw input, timestamp, and categorized JSON sub-documents (FoodData, LocationData, MoodData, MedicationData).

## 4. Non-Functional Requirements
* **Extensibility:** The data model must gracefully handle new types of tracking data without requiring constant relational schema migrations.
* **Performance:** The user-facing API must return immediately after accepting the daily input. All heavy lifting (LLM parsing, third-party API orchestration) must occur entirely in the background.
