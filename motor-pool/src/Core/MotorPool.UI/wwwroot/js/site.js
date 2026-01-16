const convertEnterpriseDatesToLocalTime = () => {

    const enterpriseEntityDateCells = document.querySelectorAll(".enterprise-entity-date")
    enterpriseEntityDateCells.forEach(cell => {
        cell.innerText = new Date(cell.innerText).toLocaleString('de-DE', {
            weekday: 'long',
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
            hour: '2-digit',
            minute: '2-digit',
            hour12: false
        });
    })

}

$('input[data-filter-property]').on('input', function () {
    const triggeredInput = $(this);
    const entityRowsSelector = `tr[data-entity="${triggeredInput.data('filter-entity')}"]`;
    $(entityRowsSelector).each(function () {
        const currentRow = $(this);

        if (triggeredInput.val().length === 0) {
            currentRow.show();
            return true;
        }

        const filteredPropertyValue = currentRow.children(`td[data-property="${triggeredInput.data('filter-property')}"]`).text().trim();
        if (!filteredPropertyValue.startsWith(triggeredInput.val())) currentRow.hide();
    });
});


document.addEventListener("DOMContentLoaded", () => {
    convertEnterpriseDatesToLocalTime()
})
