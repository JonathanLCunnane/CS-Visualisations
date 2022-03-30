
# WORKING METHOD.
import random
def generate_random_list():  # Generates a list of a size between 1 and 20 of integers between 0 and 1000.
    items = list()
    for i in range(random.randint(5, 20)):
        items.append(random.randint(0, 1000))
    
    return items


def merge_sort(items):  # The merge sort algorithm itself.
    if len(items) > 1:
        midpoint = len(items) // 2
        left_half = items[:midpoint]
        right_half = items[midpoint:]

        merge_sort(left_half)
        merge_sort(right_half)

        i = 0  # Left half iterator
        j = 0  # Right half iterator

        k = 0  # Main list iterator

        while i < len(left_half) and j < len(right_half):
            if left_half[i] <= right_half[j]:
                items[k] = left_half[i]
                i += 1
            else:
                items[k] = right_half[j]
                j += 1
            k += 1

        # For the remaining numbers (in the case of the halves not being the same length.)
        while i < len(left_half):
            items[k] = left_half[i]
            i += 1
            k += 1
        
        while j < len(right_half):
            items[k] = right_half[j]
            j += 1
            k += 1


items = generate_random_list()
print(f"The generated list of items are: {items}")

merge_sort(items)

print(f"\nThe sorted list is: {items}")