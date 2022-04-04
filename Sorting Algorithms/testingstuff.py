from tkinter.tix import Tree
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

BUTTON_FONT = 'comicsans'


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


class Button:
    def __init__(self, colour, x, y, width, height, text = '', font = BUTTON_FONT, textcolour = BLACK, border_colour=BLACK, border=False, border_thicknesss=2):
        self.colour = colour
        self.x = x
        self.y = y
        self.width = width
        self.height = height
        self.text = text
        self.font = font
        self.textcolour = textcolour
        self.border_colour = border_colour
        self.border = border
        self.border_thickness = border_thicknesss

    def draw(self):
        
        if self.border:
            
            pygame.draw.rect(SCREEN, self.border_colour, (self.x - self.border_thickness, self.y - self.border_thickness, self.width + (2 * self.border_thickness), self.height + (2 * self.border_thickness)))
            pygame.draw.rect(SCREEN, self.colour, (self.x, self.y, self.width, self.height))

        
        else:
            pygame.draw.rect(SCREEN, self.colour, (self.x, self.y, self.width, self.height))

        if self.text != '':
            font = pygame.font.SysFont(self.font, 32)
            text = font.render(self.text, 1, self.textcolour)
            SCREEN.blit(text, (self.x + (self.width / 2 - text.get_width() / 2), self.y + (self.height / 2 - text.get_height() / 2)))
        
    def isOver(self, pos):
        if pos[0] > self.x and pos[0] < self.x + self.width:  # Checks the x-coordinate of the mouse is within the x-borders of the box.
            if pos[1] > self.y and pos[1] < self.y + self.height:  # Same as above but for y-coordinates.
                return True
        
        return False


def random_columns():  # Test to generate a screen of randomly sized columns. Works well so far. Generates a new 'seed' of columns every second.    
    columns = []
    for i in range(0, SCREEN_WIDTH, int(SCREEN_WIDTH / NUMBER_OF_COLUMNS)):
        height = random.randint(0, SCREEN_HEIGHT)
        columns.append(Column(i, height))
    
    return columns


def refresh_sorted_columns(columns):
    SCREEN.fill(BG_COLOUR)

    for column in columns:
        column.colour = GREEN
        column.draw_column()
    
    pygame.display.update()
    return


def refresh_items(items):  # Takes a list of items, such as columns or buttons. Must have a draw() method.
    SCREEN.fill(BG_COLOUR)
    
    for item in items:
        item.draw()

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
    # columns = random_columns()
    buttons = []

    # Create buttons
    mergeButton = Button(RED, 400, 200, 300, 100, 'Merge Sort', textcolour=WHITE, border=True, border_colour=WHITE)
    bubbleButton = Button(RED, 800, 200, 300, 100, 'Bubble Sort', textcolour=WHITE, border=True, border_colour=WHITE)

    pygame.display.update()

    buttons.append(mergeButton)
    buttons.append(bubbleButton)

    refresh_items(buttons)  # Draws the buttons onto the screen.

    while run:
        # refresh_screen(columns, buttons)

        for event in pygame.event.get():
            pos = pygame.mouse.get_pos()

            if event.type == pygame.QUIT:
                print("Program has finished running.")
                run = False
            

            if event.type == pygame.MOUSEMOTION:
                # Insert hover colour changing.
                pass
        
        pygame.display.update()

main()

