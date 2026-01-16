import {E2eConstants} from '../utils';

function openVehicleMileageReport() {
    cy.visit('/Reports');

    cy.get(`.card:has(a:contains(Vehicle mileage))`)
        .find('a[class="report-link"]')
        .click({force: true});

    cy.url().should('include', '/Reports/VehicleMileage');
}

function checkReportTable() {
    cy.get('table').should('exist');

    cy.get('table thead tr').within(() => {
        cy.get('th').eq(0).should('have.text', 'Date');
        cy.get('th').eq(1).should('have.text', 'Mileage');
    });

    cy.get('table tbody tr').each(($row) => {
        cy.wrap($row).within(() => {
            cy.get('td').eq(0).should('not.be.empty');
            cy.get('td').eq(1).should('not.be.empty');
        });
    });
}

function fillReportForm(period: string) {
    const startDate = '2024-01-01';
    const endDate = new Date().toISOString().split('T')[0];
    const vehicleId = E2eConstants.VEHICLE_ID;

    cy.get('#StartTime').should('exist');
    cy.get('#EndTime').should('exist');
    cy.get('#Period').should('exist');
    cy.get('#VehicleId').should('exist');

    cy.get('#StartTime').clear().type(startDate);
    cy.get('#EndTime').clear().type(endDate);
    cy.get('#Period').select(period);
    cy.get('#VehicleId').clear().type(vehicleId);

    cy.get('button[type="submit"]').contains('Generate Report').click();
}

describe('Use cases tests', () => {
    beforeEach(() => {
        cy.login();
    });

    it('Open reports and generate a daily vehicle mileage report', () => {
        openVehicleMileageReport();

        fillReportForm('Day');

        checkReportTable();
    });

    it('Open reports and generate a monthly vehicle mileage report', () => {
        openVehicleMileageReport();

        fillReportForm('Month');

        checkReportTable();
    });

    it('Open reports and generate a year vehicle mileage report', () => {
        openVehicleMileageReport();

        fillReportForm('Year');

        checkReportTable();
    });
});