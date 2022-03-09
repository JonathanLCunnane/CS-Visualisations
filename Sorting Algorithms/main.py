# Sorting algorithms making use of PyGame.
# "Inspiration", or more accurately, an example of what the end goal is can be seen in videos such as this:
# https://www.youtube.com/watch?v=kPRA0W1kECg&ab_channel=TimoBingmann

from lib2to3.pgen2.token import NUMBER
import pygame
import random

pygame.init()

clock = pygame.time.Clock()

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

    def draw_column(self):  # Pygame draws from the top left hand side of the screen, this is the engine's 'origin' in terms of coordinates.
        pygame.draw.rect(SCREEN, self.colour, (self.position, SCREEN_HEIGHT - self.height, COLUMN_WIDTH, self.height))
        # Due to this, everything needs to be offset downwards, as the shapes are drawn
        # the top downwards, rather than what you would imagine, which is from the bottom to the top.
        # This is the reason for the top right position being set to 'SCREEN_HEIGHT -height'.


def random_columns():  # Test to generate a screen of randomly sized columns. Works well so far. Generates a new 'seed' of columns every second.    
    columns = []
    for i in range(0, SCREEN_WIDTH, int(SCREEN_WIDTH / NUMBER_OF_COLUMNS)):
        height = random.randint(0, SCREEN_HEIGHT)
        columns.append(Column(i, height))
    
    return columns


def bubble_sort(items):
    columns_sorted = True

    for index in range(len(items) - 1):
        if items[index].height > items[index + 1].height:
            columns_sorted = False
            temp = items[index + 1]  # Copy of the 2nd column.
            items[index].colour = RED # Changes colour of the 1st column.
            items[index + 1] = items[index] # Replaces the 2nd column with the 1st column.
            
            refresh_columns(items)  # Update screen.
            items[index + 1].colour = COLUMN_COLOUR # Changes colour of the new 2nd column.
            
            refresh_columns(items)  # Update screen.
            items[index] = temp # Replaces the 1st column with the previous 2nd column.
        
        

    for i in range(0, SCREEN_WIDTH, int(SCREEN_WIDTH / NUMBER_OF_COLUMNS)):  # Changes the position attribute of the columns to match their
        items[i // (SCREEN_WIDTH // NUMBER_OF_COLUMNS)].position = i # new pos.
        
    return items, columns_sorted


def refresh_sorted_columns(columns):
    SCREEN.fill(BG_COLOUR)

    for column in columns:
        column.colour = GREEN
        column.draw_column()
    
    pygame.display.update()


def refresh_columns(columns):
    SCREEN.fill(BG_COLOUR)
    
    for column in columns:
        column.draw_column()
    
    pygame.display.update()
    
    return


def display_column_order(columns):
    heights = []

    for column in columns:
        heights.append(column.height)
    
    print(f"The heights of the columns are {heights}.")

    return


def main():
    run = True
    columns = random_columns()

    while run:
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                print("Program has finished running.")
                run = False
            
            elif event.type == pygame.KEYDOWN:
                if event.key == pygame.K_LEFT:
                    columns_sorted = False
                    
                    print("Key pressed.")
                    
                    while not columns_sorted:
                        columns, columns_sorted = bubble_sort(columns)
                        refresh_columns(columns)
                    
                    refresh_sorted_columns(columns)
                
                if event.key == pygame.K_RIGHT:
                    display_column_order(columns)
            
        refresh_columns(columns)
        pygame.display.update()
        clock.tick(FPS)
        
    pygame.quit()

main()