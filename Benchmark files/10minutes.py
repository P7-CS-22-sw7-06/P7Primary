from numpy import matrix, dot
import random
import time

# Start the timer
start_time = time.time()

# Generate a list of 5x5 matrices
matrices = []
for i in range(15000000):
  # Create a 5x5 matrix with random values
  matrix = [[random.randint(1, 10000) for j in range(5)] for k in range(5)]
  # Add the matrix to the list
  matrices.append(matrix)

# Multiply the matrices together
result = matrices[0]
for i in range(1, len(matrices)):
    result = dot(result, matrices[i])
    # Modulo each value in the result matrix with 100000
    result = [[value % 100000 for value in row] for row in result]
    # print(result)

# Stop the timer
end_time = time.time()

# Calculate the elapsed time
elapsed_time = end_time - start_time

# Print the result
print(result)

# Print the elapsed time
print(elapsed_time)
