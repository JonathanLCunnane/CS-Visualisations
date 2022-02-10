
# WORKING METHOD.
import random
def generate_random_list():  # Generates a list of a size between 1 and 20 of integers between 0 and 1000.
    items = list()
    for i in range(random.randint(5, 20)):
        items.append(random.randint(0, 1000))
    
    return items


def bubble_sort(items):  # The bubble sort algorithm itself.
    passes = 0

    for pass_length in range(len(items), 0, -1):  # Each time this loops, a "pass" is made, each time shorter than the one before.
        index = 0
        passes += 1
        current_items = items.copy()
        while index < pass_length - 1:  
            if items[index] > items[index + 1]:
                temp = items[index + 1]
                items[index + 1] = items[index]
                items[index] = temp
            index += 1
        
        if current_items == items: # Used for optimacy. If no swaps are made, then the algorithm ends as the array is ordered.
            break
    
    print("{} passes were made.".format(passes))
    
    return items


items = generate_random_list()
print(f"The generated list of items are: {items}")

items = bubble_sort(items)

print(f"\nThe sorted list is: {items}")

