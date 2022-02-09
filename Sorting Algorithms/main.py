# Sorting algorithms making use of PyGame.
# "Inspiration", or more accurately, an example of what the end goal is can be seen in videos such as this:
# https://www.youtube.com/watch?v=kPRA0W1kECg&ab_channel=TimoBingmann


import pygame
import random
import time

pygame.init()

SCREEN_WIDTH = 800
SCREEN_HEIGHT = 500

COLUMN_WIDTH = SCREEN_WIDTH / 100  # Allows 100 rectangles / columns to be fit within the current screen size.

BLACK = (0, 0, 0)  # Colour constants.
WHITE = (255, 255, 255)
RED = (255, 0, 0)
GREEN = (0, 255, 0)
BLUE = (0, 0, 255)


SCREEN = pygame.display.set_mode((SCREEN_WIDTH, SCREEN_HEIGHT))
SCREEN.fill(BLACK)


class Column:
    def __init__(self, position, height):
        self.position = position
        self.height = height


    def update_column(self):
        draw_rectangle(self.position, self.height, self.width)


def draw_rectangle(height, position):  # Pygame draws from the top left hand side of the screen, this is the engine's 'origin' in terms of coordinates.
    pygame.draw.rect(SCREEN, WHITE, (position, SCREEN_HEIGHT - height, COLUMN_WIDTH, height))  # Due to this, everything needs to be offset downwards, as the shapes are drawn
    # the top downwards, rather than what you would imagine, which is from the bottom to the top. This is the reason for the top right position being set to 'SCREEN_HEIGHT -height'.
    

def random_columns():  # Test to generate a screen of randomly sized columns. Works well so far. Generates a new 'seed' of columns every second.    
    for i in range(0, 800, 8):
        height = random.randint(0, 500)
        draw_rectangle(height, i)
    pygame.display.flip()
    time.sleep(1)
    SCREEN.fill(BLACK)


def main():
    run = True
    while run:
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                print("Program has finished running.")
                run = False

        
        random_columns()

    pygame.quit()


main()