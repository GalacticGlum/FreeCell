COMPRESSED_CARD_GROUP_SIZE = 4
COMPRESSION_FACTOR = card_height * (2 / 100)

def card_layout(current_card_count, focus_card_index=0):
    allocations = [0] * current_card_count

    # Number of cards including the first one that can be fitted into the area without shrinking the pixel_visibility for a group of cards
    min_num_cards = ceil((window_height - card_height) / pixel_visibility)
    # Minimum pixel_visibility
    min_pixel_visibility = (window_height - card_height) / (max_cards - 1)

    excess = current_card_count - min_num_cards
    compression_visibility = max(pixel_visibility - excess * COMPRESSION_FACTOR, min_pixel_visibility)
    leftover_visibility = (window_height - card_height - compression_visibility * COMPRESSED_CARD_GROUP_SIZE) / (current_card_count - COMPRESSED_CARD_GROUP_SIZE - 1)

    for i in range(current_card_count):
        # The bottom card has no shift since there is no card after it. 
        if i == current_card_count - 1: continue
        if current_card_count <= min_num_cards:
            shift = pixel_visibility
        else:
            shift = compression_visibility if current_card_count - i <= COMPRESSED_CARD_GROUP_SIZE else leftover_visibility

        allocations[i] = shift

    return allocations

# current_card.y = previous_card.y + allocations[index]