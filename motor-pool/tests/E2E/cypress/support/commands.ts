/// <reference types="cypress" />

import { E2eConstants } from '../e2e/src/utils';

Cypress.Commands.add('login', () => {
    Cypress.log({
        name: 'login',
    });

    cy.visit('/Identity/Account/Login')
    cy.get('#Input_Email').type(E2eConstants.ADMIN_EMAIL)
    cy.get('#Input_Password').type(E2eConstants.ADMIN_PASSWORD)
    cy.get('button[type=submit]').click()

    cy.url().should('eq', `${Cypress.config().baseUrl}/`)
})