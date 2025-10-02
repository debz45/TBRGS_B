import pandas as pd
import numpy as np
from sklearn.preprocessing import MinMaxScaler
from tensorflow.keras.models import Sequential
from tensorflow.keras.layers import LSTM, Dense, Dropout
import matplotlib.pyplot as plt

# -----------------------------
# 📥 Load dataset
# -----------------------------
csv_path = r"Datasets\scats_oct2006.csv"

# Skip the first row which is an extra header
df = pd.read_csv(csv_path, skiprows=1)

# Remove any unwanted 'Unnamed' columns
df = df.loc[:, ~df.columns.str.contains('^Unnamed')]

print("✅ Dataset loaded")
print(df.head())

# -----------------------------
# 🔧 Preprocessing
# -----------------------------
# Columns for 15-min intervals: V00–V95
interval_cols = [f"V{i:02d}" for i in range(96)]

# Melt wide format to long format
df_long = df.melt(
    id_vars=["SCATS Number", "Date", "NB_LATITUDE", "NB_LONGITUDE"],
    value_vars=interval_cols,
    var_name="Interval",
    value_name="TrafficVolume"
)

# Convert Interval (V00..V95) to actual time
def interval_to_time(interval_str):
    idx = int(interval_str[1:])
    hours = idx // 4
    minutes = (idx % 4) * 15
    return f"{hours:02d}:{minutes:02d}"

df_long["Time"] = df_long["Interval"].apply(interval_to_time)
df_long["Datetime"] = pd.to_datetime(df_long["Date"] + " " + df_long["Time"], dayfirst=True)
df_long = df_long.sort_values(["SCATS Number", "Datetime"]).reset_index(drop=True)

# -----------------------------
# 🏷 Select single site for now
# -----------------------------
# Ensure SCATS Number is string
df_long["SCATS Number"] = df_long["SCATS Number"].astype(str)
site_id = "970"  # match your CSV value
site_df = df_long[df_long["SCATS Number"] == site_id].copy()
site_df = site_df[["Datetime", "TrafficVolume"]]

if site_df.empty:
    raise ValueError(f"No data found for SCATS Number {site_id}. Check site_id value!")

# -----------------------------
# 🧮 Normalize data
# -----------------------------
scaler = MinMaxScaler(feature_range=(0,1))
scaled_data = scaler.fit_transform(site_df["TrafficVolume"].values.reshape(-1,1))

# -----------------------------
# 🔄 Create sequences for LSTM
# -----------------------------
def create_sequences(data, seq_length=60):
    X, y = [], []
    for i in range(len(data) - seq_length):
        X.append(data[i:i+seq_length])
        y.append(data[i+seq_length])
    return np.array(X), np.array(y)

SEQ_LENGTH = 60
X, y = create_sequences(scaled_data, SEQ_LENGTH)
X = X.reshape((X.shape[0], X.shape[1], 1))  # [samples, timesteps, features]

# -----------------------------
# ⏱ Train/test split (time-based)
# -----------------------------
split_idx = int(len(X) * 0.8)
X_train, X_test = X[:split_idx], X[split_idx:]
y_train, y_test = y[:split_idx], y[split_idx:]

# -----------------------------
# 🔮 Build LSTM model
# -----------------------------
model = Sequential([
    LSTM(50, return_sequences=True, input_shape=(SEQ_LENGTH, 1)),
    Dropout(0.2),
    LSTM(50, return_sequences=False),
    Dropout(0.2),
    Dense(25),
    Dense(1)
])

model.compile(optimizer="adam", loss="mean_squared_error")
print("🚀 Training LSTM model...")
history = model.fit(X_train, y_train, validation_data=(X_test, y_test), epochs=10, batch_size=32)

# -----------------------------
# 🔮 Predictions
# -----------------------------
predictions = model.predict(X_test)
predictions = scaler.inverse_transform(predictions)
y_test_scaled = scaler.inverse_transform(y_test.reshape(-1,1))

# -----------------------------
# 📊 Plot results
# -----------------------------
plt.figure(figsize=(12,6))
plt.plot(site_df["Datetime"].iloc[split_idx + SEQ_LENGTH:], y_test_scaled, label="Actual Traffic")
plt.plot(site_df["Datetime"].iloc[split_idx + SEQ_LENGTH:], predictions, label="Predicted Traffic")
plt.title(f"Traffic Volume Prediction for SCATS {site_id}")
plt.xlabel("Datetime")
plt.ylabel("Traffic Volume")
plt.legend()
plt.show()
