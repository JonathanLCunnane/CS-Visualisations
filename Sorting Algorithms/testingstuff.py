from ast import Assign
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

BUTTON_FONT = 'agency fb'


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
    def __init__(self, colour, x, y, width, height, text = '', font = BUTTON_FONT, textcolour = BLACK, border_colour=BLACK, border=False, border_thicknesss=2,  function=''):
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
        self.function = function

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


def bubble_sort(items):
    columns_sorted = True

    for index in range(len(items) - 1):
        if items[index].height > items[index + 1].height:
            columns_sorted = False
            temp = items[index + 1]  # Copy of the 2nd column.
            items[index].colour = RED # Changes colour of the 1st column.
            items[index + 1] = items[index] # Replaces the 2nd column with the 1st column.
            
            refresh_items(items)  # Update screen.
            items[index + 1].colour = COLUMN_COLOUR # Changes colour of the new 2nd column.
            
            refresh_items(items)  # Update screen.
            items[index] = temp # Replaces the 1st column with the previous 2nd column.
        
        

    for i in range(0, SCREEN_WIDTH, int(SCREEN_WIDTH / NUMBER_OF_COLUMNS)):  # Changes the position attribute of the columns to match their
        items[i // (SCREEN_WIDTH // NUMBER_OF_COLUMNS)].position = i # new pos.
        
    return items, columns_sorted


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
            if left_half[i].height <= right_half[j].height:
                items[k] = left_half[i]
                refresh_items(items)
                i += 1
            else:
                items[k] = right_half[j]
                refresh_items(items)
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
    
    for z in range(0, len(items)):  # Changes the position attribute of the columns to match their
        items[z // (SCREEN_WIDTH // NUMBER_OF_COLUMNS)].position = z # new pos.

    refresh_items(items)


def main():
    run = True
    columns = random_columns()
    buttons = []
    launched = False

    # Create buttons
    mergeButton = Button(RED, 400, 200, 300, 100, text='Merge Sort', textcolour=WHITE, border=True, border_colour=WHITE)
    bubbleButton = Button(RED, 800, 200, 300, 100, text='Bubble Sort', textcolour=WHITE, border=True, border_colour=WHITE)

    pygame.display.update()

    buttons.append(mergeButton)
    buttons.append(bubbleButton)

    refresh_items(buttons)  # Draws the buttons onto the screen.

    while run:
        # refresh_screen(columns, buttons)

        for event in pygame.event.get():
            pos = pygame.mouse.get_pos()

            if event.type == pygame.QUIT:
                try:
                    print(f"{count} passes were made.")
                except UnboundLocalError:
                    print("Program was closed before sort was complete.")
                    
                print("Program has finished running.")
                run = False
            if not launched:
                if event.type == pygame.MOUSEMOTION:
                
                    for button in buttons:  # Checks if the mouse is over any of the buttons, causes it to change to green.
                        if button.isOver(pos):
                            button.colour = GREEN
                        else:
                            button.colour = RED

                    refresh_items(buttons)

                if event.type == pygame.MOUSEBUTTONDOWN:
                    for button in buttons:
                        if button.isOver(pos):
                            launched = True
                            SCREEN.fill(BLACK)
                            refresh_items(columns)
                            columns_sorted = False
                            count = 0
                            sort_method = button.text
                            print(button.text)
                
            else:  # If the game HAS been launched..
                
                if sort_method == 'Bubble Sort':
                    while not columns_sorted:
                        print(sort_method)
                        columns, columns_sorted = bubble_sort(columns)
                        count += 1

                    # Columns should be sorted now, so change all to green.

                    for column in columns:
                        column.colour = GREEN                    
                    refresh_items(columns)


                elif sort_method == 'Merge Sort':
                    merge_sort(columns)

                        
                refresh_items(columns)
                pygame.display.update()
                clock.tick(FPS)

        pygame.display.update()

main()

