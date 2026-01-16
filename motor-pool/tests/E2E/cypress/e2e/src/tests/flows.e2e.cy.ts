import {E2eConstants} from '../utils';

describe('Use cases tests', () => {
    beforeEach(() => {
        cy.login();
    });

    it('Open enterprises, choose one available and open vehicle details', () => {
        cy.get('a.nav-link').contains('Enterprises').click();
        cy.get('.card').should('exist').and('have.length.greaterThan', 0);

        cy.get(`.card:has(.card-title:contains(${E2eConstants.ENTERPRISE_NAME}))`)
            .find('a[href*="Vehicles"]')
            .click({force: true});


        cy.url().should('include', '/Vehicles');
        cy.get('h1').should('contain.text', E2eConstants.ENTERPRISE_NAME);

        cy.get('table thead tr').within(() => {
            cy.get('th').eq(0).should('contain.text', 'Company name');
            cy.get('th').eq(1).should('contain.text', 'Model name');
            cy.get('th').eq(2).should('contain.text', 'VIN');
            cy.get('th').eq(3).should('contain.text', 'Manufacture year');
            cy.get('th').eq(4).should('contain.text', 'Manufacture land');
            cy.get('th').eq(5).should('contain.text', 'Cost');
            cy.get('th').eq(6).should('contain.text', 'Mileage (km)');
            cy.get('th').eq(7).should('contain.text', 'Acquired on');
            cy.get('th').eq(8).should('contain.text', 'All trips amount');
        });

        cy.get('table tbody tr').should('exist').and('have.length.greaterThan', 0);

        cy.get('table tbody tr').each(($row) => {
            cy.wrap($row).within(() => {
                cy.get('td').eq(0).should('not.be.empty');
                cy.get('td').eq(1).should('not.be.empty');
                cy.get('td').eq(2).should('not.be.empty');
                cy.get('td').eq(3).should('not.be.empty');
                cy.get('td').eq(4).should('not.be.empty');
                cy.get('td').eq(5).should('not.be.empty');
                cy.get('td').eq(6).should('not.be.empty');
                cy.get('td').eq(7).should('not.be.empty');
                cy.get('td').eq(8).should('not.be.empty');
            });
        });

        cy.get('table tbody tr').first().within(() => {
            cy.get('a[title="Details"]').click();
        });
        cy.url().should('include', '/Vehicles/Details');
    });

    it('Open enterprises, choose one available and get all drivers', () => {
        cy.get('a.nav-link').contains('Enterprises').click();
        cy.get('.card').should('exist').and('have.length.greaterThan', 0);

        cy.get(`.card:has(.card-title:contains(${E2eConstants.ENTERPRISE_NAME}))`)
            .find('a[href*="Drivers"]')
            .click({force: true});

        cy.url().should('include', '/Drivers');
        cy.get('h1').should('contain.text', "Drivers");

        cy.get('table thead tr').within(() => {
            cy.get('th').eq(0).should('contain.text', 'First name');
            cy.get('th').eq(1).should('contain.text', 'Last name');
            cy.get('th').eq(2).should('contain.text', 'Salary');
        });

        cy.get('table tbody tr').should('exist').and('have.length.greaterThan', 0);

        cy.get('table tbody tr').each(($row) => {
            cy.wrap($row).within(() => {
                cy.get('td').eq(0).should('not.be.empty');
                cy.get('td').eq(1).should('not.be.empty');
                cy.get('td').eq(2).should('not.be.empty');
            });
        });
    });

    it('Open vehicle containing tips and show them on map', () => {
        cy.visit(`/Vehicles/Details/${E2eConstants.VEHICLE_ID}`);

        cy.get('.card-title').should('contain.text', 'Ford - Transit');

        cy.get('dl').within(() => {
            cy.get('dt').contains('VIN:').next().should('not.be.empty');
            cy.get('dt').contains('Cost:').next().should('not.be.empty');
            cy.get('dt').contains('Manufacture year:').next().should('not.be.empty');
            cy.get('dt').contains('Manufacture land:').next().should('not.be.empty');
            cy.get('dt').contains('Mileage (km):').next().should('not.be.empty');
            cy.get('dt').contains('Company name:').next().should('not.be.empty');
            cy.get('dt').contains('Acquired on:').next().should('not.be.empty');
        });

        cy.get('h3').should('contain.text', 'Trips');

        cy.get('main').find('form').within(() => {
            cy.get('label').contains('Start Date').should('exist');
            cy.get('label').contains('End Date').should('exist');
            cy.get('#StartDate').should('exist');
            cy.get('#EndDate').should('exist');
        });

        const startDate = '2024-01-01';
        const today = new Date().toISOString().split('T')[0];

        cy.get('#StartDate').type(startDate);
        cy.get('#EndDate').type(today);
        cy.get('main').find('form').submit();

        cy.get('form#tripForm').should('exist').within(() => {

            cy.get('label.list-group-item').within(() => {
                cy.get('span').contains('From:').should('exist');
                cy.get('span').contains('To:').should('exist');
            });

            cy.get('input[type="checkbox"]').check().should('be.checked');
            cy.get('button').contains('Show on map').click();
        });

        cy.url().should('include', '/Vehicles/Map');
        cy.get('main').should('exist');
        cy.get('div#map').should('exist').and('be.visible');
        cy.get('div#map').find('canvas').should('exist').and('be.visible');
        cy.get('div#map').find('.marker').should('exist').and('have.length.greaterThan', 0);
    });
});