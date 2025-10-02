import tkinter as tk
from tkinter import filedialog, messagebox
import pandas as pd
import matplotlib.pyplot as plt
from lstm_prediction import predict_with_lstm  # make sure this exists in your project

# Function to select dataset
def load_dataset():
    file_path = filedialog.askopenfilename(
        title="Select dataset file",
        filetypes=[("CSV files", "*.csv"), ("All files", "*.*")]
    )
    if file_path:
        try:
            df = pd.read_csv(file_path)
            messagebox.showinfo("Dataset Loaded", f"Dataset loaded successfully!\nRows: {len(df)}")
            return df
        except Exception as e:
            messagebox.showerror("Error", f"Could not load dataset: {e}")
    return None

# Function to run prediction
def run_prediction():
    df = load_dataset()
    if df is None:
        return
    
    try:
        predictions = predict_with_lstm(df)  # call your LSTM
        plt.figure(figsize=(8, 4))
        plt.plot(df.index, df.iloc[:, 0], label="Actual")   # assuming first column is target
        plt.plot(df.index[-len(predictions):], predictions, label="Predicted")
        plt.legend()
        plt.title("LSTM Prediction")
        plt.show()
    except Exception as e:
        messagebox.showerror("Prediction Error", f"Failed to run LSTM: {e}")

# Tkinter window
root = tk.Tk()
root.title("Traffic Prediction GUI")
root.geometry("400x200")

label = tk.Label(root, text="Traffic Prediction System", font=("Arial", 16))
label.pack(pady=20)

btn_load = tk.Button(root, text="Run Prediction", command=run_prediction, width=20)
btn_load.pack(pady=10)

btn_exit = tk.Button(root, text="Exit", command=root.quit, width=20)
btn_exit.pack(pady=10)

# Start the GUI loop
root.mainloop()
