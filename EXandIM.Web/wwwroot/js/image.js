// Function to dynamically add new file input to the 'head' container
function addFileInput() {
    // Get the 'head' container
    var container = document.getElementById('head');

    // Create a new div to hold the input and label
    var inputBlock = document.createElement('div');
    inputBlock.classList.add('mb-3');

    // Create label for new file input
    var label = document.createElement('label');
    label.classList.add('form-label');
    label.innerText = 'اختر صور جديدة:';

    // Create the new file input element
    var input = document.createElement('input');
    input.type = 'file';
    input.name = 'BookFiles';
    input.classList.add('form-control');
    input.multiple = true;

    // Create a span for validation messages (if any)
    var validationSpan = document.createElement('span');
    validationSpan.classList.add('text-danger');

    // Append the label, input, and validation span to the input block
    inputBlock.appendChild(label);
    inputBlock.appendChild(input);
    inputBlock.appendChild(validationSpan);

    // Append the new input block to the 'head' container
    container.appendChild(inputBlock);
}

// Function to delete an image and remove it from the UI
function deleteImage(imageUrl, imageIndex) {
    if (confirm('Are you sure you want to delete this image?')) {
        // Track deleted images in hidden field
        const deletedImagesInput = document.getElementById('DeletedFileUrls');
        let deletedFileUrls = deletedImagesInput.value ? deletedImagesInput.value.split(',') : [];
        deletedFileUrls.push(imageUrl);
        deletedImagesInput.value = deletedFileUrls.join(',');

        // Remove the image's UI element
        var imageItem = document.getElementById('image-item-' + imageIndex);
        if (imageItem) {
            imageItem.remove();
        }
    }
}