
# WORKING METHOD.
import random
def generate_random_list():  # Generates a list of a size between 1 and 20 of integers between 0 and 1000.
    items = list()
    for i in range(random.randint(5, 20)):
        items.append(random.randint(0, 1000))
    
    return items

# 
# def merge_sort(items):  # The merge sort algorithm itself.
#     if len(items) > 1:
#         midpoint = len(items) // 2
#         left_half = items[:midpoint]
#         right_half = items[midpoint:]
# 
#         merge_sort(left_half)
#         merge_sort(right_half)
# 
#         i = 0  # Left half iterator
#         j = 0  # Right half iterator
# 
#         k = 0  # Main list iterator
# 
#         while i < len(left_half) and j < len(right_half):
#             if left_half[i] <= right_half[j]:
#                 items[k] = left_half[i]
#                 i += 1
#             else:
#                 items[k] = right_half[j]
#                 j += 1
#             k += 1
# 
#         # For the remaining numbers (in the case of the halves not being the same length.)
#         while i < len(left_half):
#             items[k] = left_half[i]
#             i += 1
#             k += 1
#         
#         while j < len(right_half):
#             items[k] = right_half[j]
#             j += 1
#             k += 1
# 

import pygame

pygame.init()

SCREEN_WIDTH = 1500
SCREEN_HEIGHT = 500
NUMBER_OF_COLUMNS = 200
FPS = int(0.6 * NUMBER_OF_COLUMNS)

COLUMN_WIDTH = SCREEN_WIDTH / NUMBER_OF_COLUMNS  # Allows 100 rectangles / columns to be fit within the current screen size.

BLACK = (0, 0, 0)  # Colour constants.
WHITE = (255, 255, 255)
RED = (255, 0, 0)
GREEN = (0, 255, 0)
BLUE = (0, 0, 255)

COLUMN_COLOUR = WHITE
BG_COLOUR = BLACK

SCREEN = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))
SCREEN.fill(BG_COLOUR)


class Column:
    def __init__(self, position, height, colour=COLUMN_COLOUR):
        self.position = position
        self.height = height
        self.colour = colour

    def draw(self):  # Pygame draws from the top left hand side of the screen, this is the engine's 'origin' in terms of coordinates.
        pygame.draw.rect(SCREEN, self.colour, (self.position, SCREEN_HEIGHT - self.height, COLUMN_WIDTH, self.height))
        # Due to this, everything needs to be offset downwards, as the shapes are drawn
        # the top downwards, rather than what you would imagine, which is from the bottom to the top.
        # This is the reason for the top right position being set to 'SCREEN_HEIGHT -height'.



def merge_sort(items):
    merge_sort_2(items, 0, len(items) - 1)


def merge_sort_2(items, start, end):
    if start < end:  # Means there's more than 1 item in the list.
        middle = (start + end) // 2  # Gets the midpoint

        merge_sort_2(items, start, middle)
        merge_sort_2(items, middle + 1, end)


        merge(items, start, middle, end)

    

def merge(items, start, middle, end):
    
    left = items[start:middle + 1]
    right = items[middle + 1: end + 1]

    left.append(9999999999999)  # Value that allows us to know when the end of the list is reached.
    right.append(9999999999999)

    i = j = 0

    for k in range(start, end + 1):
        if left[i] <= right[j]:  # If the left side is smaller, it is added to the array and its index is increased.
            items[k] = left[i]
            i += 1
        else:
            items[k] = right[j]
            j += 1

items = generate_random_list()
print(items)

merge_sort(items)

print(items)


